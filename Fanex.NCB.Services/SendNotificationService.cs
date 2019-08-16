namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using RestSharp;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;
        private readonly IOptionsSnapshot<AppSettings> appSettings;

        public SendNotificationService(IChatbotService chatbotService,
            IBaseService baseService,
            IOptionsSnapshot<AppSettings> appSettings)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.appSettings = appSettings;
        }

        public async Task ScanAndSendNotification()
        {
            var notifications = await GetNotificationPlannedToSend().ConfigureAwait(false);

            await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));

            await SendNotificationToRecipients(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend))).ConfigureAwait(false);
        }

        private async Task UpdateFailedRecipientStatus(IEnumerable<NotificationRecipient> recipients)
        {
            if (recipients.Any())
            {
                var updateNoti = recipients.Select(r => new UpdateNotificationModel
                {
                    NotificationID = r.NotificationId,
                    RecipientID = r.RecipientId,
                    SentTime = DateTimeExtension.ConvertToDateTimeString(DateTime.Now.ConvertToUTC()),
                    ConversationGroupID = r.ChatGroupId,
                    ConversationBotID = r.BotId,
                    Retry = false,
                    IsSuccess = false,
                    ErrMsg = EnumHelper.GetDescription(r.RecipientStatus)
                });

                await UpdateSentNotificationStatus(updateNoti);
            }
        }

        private async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationPlannedToSend()
        {
            var criteria = new GetNotificationsToSendCriteria();
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria)
                                .ConfigureAwait(false);
            return notifications;
        }

        private static IList<SendMessageResult> ConvertJsonResponse(string json)
        {
            var messages = JsonConvert.DeserializeObject<List<SendMessageResult>>(json);

            return messages;
        }

        private async Task SendNotificationToRecipients(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            int itemEachBatch = appSettings.Value.ItemEachBatchSendNotification;
            var notiMsgs = GenerateNotificationMessage(notifications, recipients);

            int totalBatch = (int)Math.Ceiling((double)notiMsgs.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                var currentMsgs = notiMsgs.Skip(i * itemEachBatch).Take(itemEachBatch);
                var response = await chatbotService.SendMessageToMultitpleAsync(currentMsgs).ConfigureAwait(false);
                await HandleResponse(response, recipients).ConfigureAwait(false);
            }
        }

        private async Task HandleResponse(IRestResponse response, IEnumerable<NotificationRecipient> recipients)
        {
            if (response.IsSuccessful && response.StatusCode.Equals(HttpStatusCode.OK))
            {
                var responseMsgs = ConvertJsonResponse(response.Content);
                await UpdateStatusAfterSendNotification(recipients, responseMsgs);
            }
            else
            {
                foreach (var recipient in recipients)
                {
                    recipient.ErrCode = (int)RecipientStatuses.BotServiceError;
                }

                await UpdateFailedRecipientStatus(recipients).ConfigureAwait(false);
            }
        }

        private async Task UpdateStatusAfterSendNotification(IEnumerable<NotificationRecipient> recipients, IEnumerable<SendMessageResult> responseMsgs)
        {
            var updateNotiModels = new List<UpdateNotificationModel>();
            foreach (var recipient in recipients)
            {
                var result = responseMsgs.FirstOrDefault(r => r.NotificationId == recipient.NotificationId && r.ConversationId == recipient.ChatGroupConversationId);
                var model = new UpdateNotificationModel
                {
                    NotificationID = recipient.NotificationId,
                    RecipientID = recipient.RecipientId,
                    SentTime = DateTimeExtension.ConvertToDateTimeString(result.SentTime),
                    ConversationGroupID = recipient.ChatGroupId,
                    ConversationBotID = recipient.BotId,
                    Retry = result.ErrorCode != 0,
                    IsSuccess = result.ErrorCode == 0,
                    ErrMsg = result.ErrorCode == 0 ? string.Empty : result.ErrorMessage
                };
                updateNotiModels.Add(model);
            }

            await UpdateSentNotificationStatus(updateNotiModels).ConfigureAwait(false);
        }

        private IEnumerable<NotificationMessage> GenerateNotificationMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var msgList = new List<NotificationMessage>();
            foreach (var noti in notifications)
            {
                var msg = new NotificationMessage
                {
                    NotifictaionId = noti.NotificationId,
                    Message = string.Format("{0}\n{1}", noti.Title, noti.Content),
                    Recipients = recipients.Where(r => noti.NotificationId == r.NotificationId)
                            .Select(r => new ConversationInfo
                            {
                                BotConversationId = r.BotConversationId,
                                ServiceUrl = r.ServiceUrl,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversationId,
                                BotName = r.BotName
                            }).Distinct()
                };
                msgList.Add(msg);
            }

            return msgList;
        }

        private async Task UpdateSentNotificationStatus(IEnumerable<UpdateNotificationModel> msgs)
        {
            if (msgs.Any())
            {
                string stringMsg = JsonConvert.SerializeObject(msgs);
                var command = new UpdateRecipientSendNotifiactionStatusCommand
                {
                    SentStatus = stringMsg
                };

                await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
            }
        }

        public async Task SendNotificationImmediately(int notifiactionId)
        {
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = notifiactionId
            };
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria)
                .ConfigureAwait(false);

            await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)
            && !r.RecipientStatus.Equals(RecipientStatuses.BotServiceError)));

            await SendNotificationToRecipients(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend))).ConfigureAwait(false);
        }
    }
}