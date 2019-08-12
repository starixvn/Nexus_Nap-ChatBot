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
    using Newtonsoft.Json.Linq;
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
                    NotificationId = r.NotificationId,
                    RecipientId = r.RecipientId,
                    SentTime = DateTime.Now,
                    ConversationGroupId = r.ChatGroupId,
                    ConversationBotId = r.BotId,
                    IsSuccess = false,
                    SentStatus = r.RecipientStatus,
                    ErrMsg = EnumHelper.GetDescription(r.RecipientStatus)
                });

                await UpdateSentNotificationStatus(updateNoti);
            }
        }

        private async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationPlannedToSend()
        {
            var criteria = new GetNotificationsToSendCriteria();
            return await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria).ConfigureAwait(false);
        }

        private IList<ProactiveMessageError> ConvertJsonResponse(string json)
        {
            var messages = new List<ProactiveMessageError>();
            JObject obj = JObject.Parse(json);
            var results = obj["results"];
            foreach (var result in results)
            {
                var proactiveMessage = new ProactiveMessageError
                {
                    NotificationId = (int)result["notificationId"],
                    ConversationId = (string)result["conversationId"],
                    ErrorCode = (int)result["errorCode"],
                    ErrorMessage = (string)result["errorMessage"]
                };
                messages.Add(proactiveMessage);
            }

            return messages;
        }

        private async Task SendNotificationToRecipients(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            int itemEachBatch = appSettings.Value.ItemEachBatchSendNotification;
            var proactiveMsgs = GenerateProactiveMessage(notifications, recipients);

            int totalBatch = (int)Math.Ceiling((double)proactiveMsgs.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                var currentMsgs = proactiveMsgs.Skip(i * itemEachBatch).Take(itemEachBatch);
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
                    recipient.SendingError = (int)RecipientStatuses.BotServiceError;
                }

                await UpdateFailedRecipientStatus(recipients).ConfigureAwait(false);
            }
        }

        private async Task UpdateStatusAfterSendNotification(IEnumerable<NotificationRecipient> recipients, IList<ProactiveMessageError> responseMsgs)
        {
            var updateNotiModels = new List<UpdateNotificationModel>();
            foreach (var recipient in recipients)
            {
                int code = responseMsgs.FirstOrDefault().ErrorCode;
                var model = new UpdateNotificationModel
                {
                    NotificationId = recipient.NotificationId,
                    RecipientId = recipient.RecipientId,
                    SentTime = DateTime.Now,
                    ConversationGroupId = recipient.ChatGroupId,
                    ConversationBotId = recipient.BotId,
                    IsSuccess = code == 0,
                    SentStatus = code == 0 ? RecipientStatuses.ReadyToSend : RecipientStatuses.BotServiceError,
                    ErrMsg = code == 0 ? string.Empty : EnumHelper.GetDescription((RecipientStatuses)code)
                };
                updateNotiModels.Add(model);
            }

            await UpdateSentNotificationStatus(updateNotiModels).ConfigureAwait(false);
        }

        private IEnumerable<ReciveProactiveMessage> GenerateProactiveMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var proactiveMsgs = new List<ReciveProactiveMessage>();
            foreach (var noti in notifications)
            {
                var proactiveMsg = new ReciveProactiveMessage
                {
                    NotifictaionId = noti.NotificationId,
                    Message = string.Format("{0}\n{1}", noti.Title, noti.Content),
                    Recipients = recipients.Where(r => noti.NotificationId == noti.NotificationId)
                            .Select(r => new ProactiveMessage
                            {
                                BotConversationId = r.BotConversationId,
                                ServiceUrl = r.ServiceUrl,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversation,
                                BotName = r.BotName
                            })
                };
                proactiveMsgs.Add(proactiveMsg);
            }

            return proactiveMsgs;
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

        public async Task SendNotificationImediately(int notifiactionId)
        {
            var criteria = new GetNotificationByIdCriteria
            {
                NotifictaionId = notifiactionId
            };
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria)
                .ConfigureAwait(false);

            await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));

            await SendNotificationToRecipients(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend))).ConfigureAwait(false);
        }
    }
}