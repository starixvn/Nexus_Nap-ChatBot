namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Fanex.Logging;
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
        private readonly IOptionsSnapshot<MonitorChatbot> monitorOptions;
        private readonly ILogger logger;

        public SendNotificationService(IChatbotService chatbotService,
            IBaseService baseService,
            IOptionsSnapshot<AppSettings> appSettings,
            IOptionsSnapshot<MonitorChatbot> monitorOptions,
            ILogger logger)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.appSettings = appSettings;
            this.monitorOptions = monitorOptions;
            this.logger = logger;
        }

        public async Task ScanAndSendNotification()
        {
            var notifications = await GetNotificationPlannedToSend();
            if (notifications.Item1.Any())
            {
                await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));

                await SendNotificationToRecipients(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));
            }
        }

        private async Task UpdateFailedRecipientStatus(IEnumerable<NotificationRecipient> recipients, bool isRetry = false)
        {
            if (recipients.Any())
            {
                var updateNoti = recipients.Select(r => new UpdateNotificationModel
                {
                    NotificationID = r.NotificationId,
                    RecipientID = r.RecipientId,
                    SentTime = DateTimeExtension.ConvertToDateTimeString(DateTime.UtcNow),
                    ConversationGroupID = r.ChatGroupId,
                    ConversationBotID = r.BotId,
                    Retry = isRetry,
                    IsSuccess = false,
                    ErrMsg = GenerateErrorMessage(r.RecipientStatus, r.ErrorMessage)
                });

                await UpdateSentNotificationStatus(updateNoti);
            }
        }

        private static string GenerateErrorMessage(RecipientStatuses recipientStatus, string errMsg)
        {
            return recipientStatus == RecipientStatuses.BotServiceError ?
                                        $"[Internal Error] {errMsg}" :
                                        EnumHelper.GetDescription(recipientStatus);
        }

        private async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationPlannedToSend()
        {
            var criteria = new GetNotificationsToSendCriteria();
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);
            return notifications;
        }

        private static IList<SendMessageResult> ConvertJsonResponse(string json)
        {
            var messages = JsonConvert.DeserializeObject<List<SendMessageResult>>(json);

            return messages ?? new List<SendMessageResult>();
        }

        private async Task SendNotificationToRecipients(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var notiMsgs = GenerateNotificationMessage(notifications, recipients);
            var index = 0;
            foreach (var notiMsg in notiMsgs)
            {
                index++;
                var currentRecipients = recipients.Where(r => r.NotificationId == notiMsg.NotifictaionId);
                var response = await SendMessageToMultipleWithHandleException(notiMsg, currentRecipients);
                await HandleResponse(response, currentRecipients);
                if (index != notiMsgs.Count())
                {
                    await Task.Delay(appSettings.Value.DelayTimeEachBatch);
                }
            }
        }

        private async Task<IRestResponse> SendMessageToMultipleWithHandleException(
            NotificationMessage messages,
            IEnumerable<NotificationRecipient> recipients)
        {
            try
            {
                return await chatbotService.SendMessageToMultitpleAsync(messages);
            }
            catch (Exception exception)
            {
                var updateRecipients = recipients.Select(r =>
                    {
                        return new NotificationRecipient
                        {
                            NotificationId = r.NotificationId,
                            RecipientId = r.RecipientId,
                            BotId = r.BotId,
                            ChatGroupId = r.ChatGroupId,
                            ErrCode = (int)RecipientStatuses.BotServiceError,
                            ErrorMessage = exception.Message
                        };
                    });

                await UpdateFailedRecipientStatus(updateRecipients, true);
                string errMsg = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                exception.Message,
                exception.Source,
                string.Empty,
                exception.TargetSite,
                exception.StackTrace);
                await logger.ErrorAsync(errMsg);
                throw;
            }
        }

        private async Task HandleResponse(IRestResponse response, IEnumerable<NotificationRecipient> recipients)
        {
            if (response.IsSuccessful && response.StatusCode.Equals(HttpStatusCode.OK))
            {
                var responseMessages = ConvertJsonResponse(response.Content);
                await UpdateStatusAfterSendNotification(recipients, responseMessages);
            }
            else
            {
                var updatedRecipients = recipients.Select(r =>
                {
                    return new NotificationRecipient
                    {
                        NotificationId = r.NotificationId,
                        RecipientId = r.RecipientId,
                        BotId = r.BotId,
                        ChatGroupId = r.ChatGroupId,
                        ErrCode = (int)RecipientStatuses.BotServiceError,
                        ErrorMessage = response.ErrorMessage
                    };
                });

                await UpdateFailedRecipientStatus(updatedRecipients, true);
            }
        }

        private async Task UpdateStatusAfterSendNotification(IEnumerable<NotificationRecipient> recipients, IEnumerable<SendMessageResult> responseMessages)
        {
            var updateNotiModels = new List<UpdateNotificationModel>();
            foreach (var recipient in recipients)
            {
                var result = responseMessages.FirstOrDefault(r => r.NotificationId == recipient.NotificationId
                && r.ConversationId == recipient.ChatGroupConversationId);

                if (result != null)
                {
                    var model = new UpdateNotificationModel
                    {
                        NotificationID = recipient.NotificationId,
                        RecipientID = recipient.RecipientId,
                        SentTime = DateTimeExtension.ConvertToDateTimeString(result.SentTime),
                        ConversationGroupID = recipient.ChatGroupId,
                        ConversationBotID = recipient.BotId,
                        Retry = result.ErrorCode != 0,
                        IsSuccess = result.ErrorCode == 0,
                        ErrMsg = result.ErrorCode == 0 ? string.Empty :
                            GenerateErrorMessage(RecipientStatuses.BotServiceError, result.ErrorMessage)
                    };
                    updateNotiModels.Add(model);
                }
            }

            await UpdateSentNotificationStatus(updateNotiModels);
        }

        private IEnumerable<NotificationMessage> GenerateNotificationMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var messages = new List<NotificationMessage>();

            foreach (var noti in notifications)
            {
                var currentRecipients = new List<NotificationRecipient>(recipients
                                        .Where(r => r.NotificationId == noti.NotificationId)
                                        .DistinctBy(r => r.ChatGroupConversationId));

                var batchs = DivideBatchForNotification(noti, currentRecipients);
                messages.AddRange(batchs);
            }

            return messages;
        }

        private IEnumerable<NotificationMessage> DivideBatchForNotification(NotificationSendMessage
            notification, IEnumerable<NotificationRecipient> recipients)
        {
            var listBatchs = new List<NotificationMessage>();

            int itemEachBatch = appSettings.Value.ItemEachBatchSendNotification;
            int totalBatch = (int)Math.Ceiling((double)recipients.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                var currentRecipients = recipients.Skip(i * itemEachBatch).Take(itemEachBatch);
                listBatchs.Add(new NotificationMessage
                {
                    NotifictaionId = notification.NotificationId,
                    Message = notification.Content,
                    Recipients = currentRecipients.Select(r => new ConversationInfo
                    {
                        BotConversationId = r.BotConversationId,
                        ServiceUrl = r.ServiceUrl,
                        ChannelId = r.ChannelId,
                        ConversationId = r.ChatGroupConversationId,
                        BotName = r.BotName
                    }).DistinctBy(r => r.ConversationId)
                });
            }

            listBatchs.Last().Recipients = AddMonitorRecipient(listBatchs.Last().Recipients);

            return listBatchs;
        }

        private IEnumerable<ConversationInfo> AddMonitorRecipient(IEnumerable<ConversationInfo> conversations)
        {
            var settings = monitorOptions.Value;
            if (settings.IsMonitoring)
            {
                var conversation = new ConversationInfo
                {
                    ConversationId = settings.ConversationId,
                    ChannelId = settings.ChannelId,
                    ServiceUrl = settings.ServiceUrl,
                    BotConversationId = settings.BotConversationId,
                    BotName = settings.BotName
                };

                conversations = conversations.Append(conversation);
            }

            return conversations;
        }

        private async Task UpdateSentNotificationStatus(IEnumerable<UpdateNotificationModel> messages)
        {
            if (messages.Any())
            {
                string json = JsonConvert.SerializeObject(messages);

                var command = new UpdateRecipientSendNotifiactionStatusCommand
                {
                    SentStatus = Regex.Unescape(json)
                };
                await baseService.ExecuteNonQueryAsync(command);
            }
        }

        public async Task SendNotificationImmediately(int notifiactionId)
        {
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = notifiactionId
            };
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);

            await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)
            && !r.RecipientStatus.Equals(RecipientStatuses.BotServiceError)));

            await SendNotificationToRecipients(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));
        }
    }
}