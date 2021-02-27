namespace Fanex.NCB.Services
{
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly IOptionsSnapshot<AppSettings> appSettings;
        private readonly IOptionsSnapshot<MonitorChatbot> monitorOptions;
        private readonly ILogService logService;

        public SendNotificationService(IChatbotService chatbotService,
            IBaseService baseService,
            IPushSignalService pushSignalService,
            IOptionsSnapshot<AppSettings> appSettings,
            IOptionsSnapshot<MonitorChatbot> monitorOptions,
            ILogService logService)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.appSettings = appSettings;
            this.monitorOptions = monitorOptions;
            this.logService = logService;
        }

        public async Task ScanAndSendNotification()
        {
            var notifications = await GetNotificationPlannedToSend();
            if (notifications.Item1.Any())
            {
                await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));
                var readyToSendRecipients = notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend));
                logService.WriteLogFileInfo($"Get Notification: Total recipients: {notifications.Item2.Count()}, ready to send: ");

                await SendNotificationToRecipients(notifications.Item1, readyToSendRecipients);
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
            if (!string.IsNullOrWhiteSpace(criteria.UpdatedNotificationIds))
            {
                PushSignalUpdateNotiStatus(criteria.UpdatedNotificationIds);
            }
            return notifications;
        }

        private static IList<SendMessageResult> ConvertJsonResponse(string json)
        {
            var messages = JsonConvert.DeserializeObject<List<SendMessageResult>>(json);

            return messages ?? new List<SendMessageResult>();
        }

        private static IEnumerable<NotificationRecipient> GetDistinctChatGroupConversationId(IEnumerable<NotificationRecipient> recipients)
        {
            var cloneRecipients = new List<NotificationRecipient>(recipients);
            return cloneRecipients.GroupBy(g => new { g.NotificationId, g.ChatGroupConversationId })
                .Select(group => group.FirstOrDefault()).OrderBy(r => r.NotificationId);
        }

        private async Task SendNotificationToRecipients(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var distinctRecipients = GetDistinctChatGroupConversationId(recipients);
            var batches = DivideNotificationBatch(notifications, distinctRecipients);
            var index = 0;
            foreach (var batch in batches)
            {
                index++;
                logService.WriteLogFileInfo($"Start batch {index} ");
                await SendMessageToMultipleWithHandleException(batch.Item1, batch.Item2, recipients, index);
                logService.WriteLogFileInfo($"End batch {index} ");
            }
        }

        private async Task SendMessageToMultipleWithHandleException(
            IEnumerable<NotificationMessage> messages,
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<NotificationRecipient> originRecipients,
            int index)
        {
            try
            {
                logService.WriteLogFileInfo($"Start SendMessageToMultitpleAsync for batch {index} - {recipients.Count()} recipients ");
                var response = await chatbotService.SendMessageToMultitpleAsync(messages);
                logService.WriteLogFileInfo($"End SendMessageToMultitpleAsync for batch {index} ");

                if (response.IsSuccessful && response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    logService.WriteLogFileInfo($"Start handle response success for batch {index}");
                    var responseMessages = ConvertJsonResponse(response.Content);
                    await UpdateStatusAfterSendNotification(originRecipients, responseMessages);
                }
                else
                {
                    logService.WriteLogFileInfo($"Start handle response fail for batch {index}, data: {response.ErrorMessage}");

                    await UpdateFailedStatusForBatch(recipients, originRecipients, response.ErrorMessage);

                    if (response.ErrorException != null)
                    {
                        await logService.WriteExceptionSentryErrorAsync(response.ErrorException);
                    }
                    else
                    {
                        await logService.WriteMessageSentryErrorAsync($"Send Message fail: {JsonConvert.SerializeObject(response)}");
                        logService.WriteLogFileInfo($"SendMessageToMutipleAsync fail: batch {index}, data: {JsonConvert.SerializeObject(response)}");
                    }
                }

                logService.WriteLogFileInfo($"End handle response for batch {index}");
            }
            catch (Exception exception)
            {
                logService.WriteLogFileInfo($"SendMessageToMutipleAsync throws exception: batch {index}, data: {JsonConvert.SerializeObject(messages)}");
                await UpdateFailedStatusForBatch(recipients, originRecipients, exception.Message);
                await logService.WriteExceptionSentryErrorAsync(exception);
            }
        }

        private async Task UpdateFailedStatusForBatch(IEnumerable<NotificationRecipient> recipients, IEnumerable<NotificationRecipient> originRecipients, string errorMessage)
        {
            var updateNotiModels = new List<UpdateNotificationModel>();
            foreach (var recipient in originRecipients)
            {
                var results = recipients.Where(r => r.NotificationId == recipient.NotificationId
                && r.ChatGroupConversationId == recipient.ChatGroupConversationId);

                foreach (var result in results)
                {
                    var model = new UpdateNotificationModel
                    {
                        NotificationID = recipient.NotificationId,
                        RecipientID = recipient.RecipientId,
                        SentTime = DateTimeExtension.ConvertToDateTimeString(DateTime.UtcNow),
                        ConversationGroupID = recipient.ChatGroupId,
                        ConversationBotID = recipient.BotId,
                        Retry = true,
                        IsSuccess = false,
                        ErrMsg = GenerateErrorMessage(RecipientStatuses.BotServiceError, errorMessage)
                    };
                    updateNotiModels.Add(model);
                }
            }

            await UpdateSentNotificationStatus(updateNotiModels);
        }

        private async Task UpdateStatusAfterSendNotification(IEnumerable<NotificationRecipient> recipients, IEnumerable<SendMessageResult> responseMessages)
        {
            var updateNotiModels = new List<UpdateNotificationModel>();
            foreach (var recipient in recipients)
            {
                var results = responseMessages.Where(r => r.NotificationId == recipient.NotificationId
                && r.ConversationId == recipient.ChatGroupConversationId);

                foreach (var result in results)
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

        private IEnumerable<ValueTuple<List<NotificationMessage>, List<NotificationRecipient>>>
            DivideNotificationBatch(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var batches = new List<ValueTuple<List<NotificationMessage>, List<NotificationRecipient>>>();
            var batchRecipients = new List<List<NotificationRecipient>>();
            int itemEachBatch = appSettings.Value.ItemEachBatchSendNotification;
            int totalBatch = (int)Math.Ceiling((double)recipients.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                batchRecipients.Add(recipients.Skip(i * itemEachBatch).Take(itemEachBatch).ToList());
            }

            foreach (var batch in batchRecipients)
            {
                var listNotificationIds = batch.Select(p => p.NotificationId).Distinct();
                var newBatches = new List<NotificationMessage>(notifications
                                .Where(noti => listNotificationIds.Any(l => l == noti.NotificationId))
                                .Select(n => new NotificationMessage
                                {
                                    NotificationId = n.NotificationId,
                                    Message = n.Content
                                }));

                foreach (var item in newBatches)
                {
                    item.Recipients = new List<ConversationInfo>(batch.Where(r => r.NotificationId == item.NotificationId)
                        .Select(r => new ConversationInfo
                        {
                            BotConversationId = r.BotConversationId,
                            ServiceUrl = r.ServiceUrl,
                            ChannelId = r.ChannelId,
                            ConversationId = r.ChatGroupConversationId,
                            BotName = r.BotName
                        }).DistinctBy(r => r.ConversationId));
                }
                batches.Add(new ValueTuple<List<NotificationMessage>, List<NotificationRecipient>>(newBatches, batch));
            }

            batches.Last().Item1.Last().Recipients = AddMonitorRecipient(batches.Last().Item1.Last().Recipients);
            return batches;
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
                    SentStatus = json
                };
                await baseService.ExecuteNonQueryAsync(command);

                if (!string.IsNullOrWhiteSpace(command.UpdatedNotificationIds))
                {
                    PushSignalUpdateNotiStatus(command.UpdatedNotificationIds);
                }

                if (command.IsHaveErrNotification)
                {
                    PushSignalNewErrNotification();
                }
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

        private void PushSignalUpdateNotiStatus(string notificationIds)
        {
            Task.Run(() => pushSignalService.OnUpdateNotificationStatus(notificationIds));
        }

        private void PushSignalNewErrNotification()
        {
            Task.Run(() => pushSignalService.OnHasNewErrNotifications());
        }
    }
}