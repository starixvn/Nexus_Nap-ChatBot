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

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly AppSettings appSettings;
        private readonly MonitorChatbot monitorChatBot;
        private readonly ILogService logService;
        private readonly MonitorTelegramBot monitorTelegramBot;

        public SendNotificationService(
            IChatbotService chatbotService,
            IBaseService baseService,
            IPushSignalService pushSignalService,
            ILogService logService,
            IOptionsSnapshot<AppSettings> appSettingOptions,
            IOptionsSnapshot<MonitorChatbot> monitorChatBotOptions,
            IOptionsSnapshot<MonitorTelegramBot> monitorTelegramBotOptions)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.logService = logService;
            this.appSettings = appSettingOptions.Value;
            this.monitorChatBot = monitorChatBotOptions.Value;
            this.monitorTelegramBot = monitorTelegramBotOptions.Value;
        }

        public async Task ScanAndSendNotificationAsync()
        {
            var (notifications, recipients) = await GetNotificationPlannedToSendAsync();
            if (notifications.Any())
            {
                await UpdateFailedRecipientStatusAsync(recipients.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)).ToList());
                var readyToSendRecipients = recipients.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)).ToList();

                await SendNotificationToRecipients(notifications, readyToSendRecipients);
            }
        }

        public async Task SendNotificationImmediatelyAsync(int notifiactionId)
        {
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = notifiactionId
            };
            var (notifications, recipients) = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);

            await UpdateFailedRecipientStatusAsync(recipients.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)
                    && !r.RecipientStatus.Equals(RecipientStatuses.BotServiceError)).ToList());

            var readyToSendRecipients = recipients.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)).ToList();
            await SendNotificationToRecipients(notifications, readyToSendRecipients);
        }

        protected internal virtual async Task UpdateFailedRecipientStatusAsync(IEnumerable<NotificationRecipient> recipients, bool isRetry = false)
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

                await UpdateSentNotificationStatusAsync(updateNoti);
            }
        }

        private static string GenerateErrorMessage(RecipientStatuses recipientStatus, string errMsg)
        {
            var errorMsgs = new[]
            {
                Resources.Resources.DisableChannelErrorMessage,
                Resources.Resources.DuplicatedErrorMessage
            };
            if (errorMsgs.Contains(errMsg))
            {
                return errMsg;
            }

            return recipientStatus == RecipientStatuses.BotServiceError
                ? $"[Internal Error] {errMsg}" : EnumHelper.GetDescription(recipientStatus);
        }

        protected internal virtual async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationPlannedToSendAsync()
        {
            var criteria = new GetNotificationsToSendCriteria();
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);
            if (!string.IsNullOrWhiteSpace(criteria.UpdatedNotificationIds))
            {
                PushSignalUpdateNotiStatus(criteria.UpdatedNotificationIds);
            }

            return notifications;
        }

        private static IEnumerable<NotificationRecipient> GetDistinctChatGroupConversationId(IEnumerable<NotificationRecipient> recipients)
        {
            var cloneRecipients = new List<NotificationRecipient>(recipients);
            return cloneRecipients.GroupBy(g => new { g.NotificationId, g.ChatGroupConversationId })
                .Select(group => group.FirstOrDefault()).OrderBy(r => r.NotificationId).ToList();
        }

        protected internal virtual async Task SendNotificationToRecipients(
            IEnumerable<NotificationSendMessage> notifications,
            IEnumerable<NotificationRecipient> recipients)
        {
            await Task.WhenAll(
                SendMessageByChannelAsync((byte)ConversationChannel.Skype, notifications, recipients),
                SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notifications, recipients)
            );
        }

        private async Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var filterNotifications = notifications.Where(x => x.ChannelId == channelId).ToList();
            var filterRecipients = recipients.Where(x => x.ChannelId == channelId && filterNotifications.Any(y => y.NotificationId == x.NotificationId)).ToList();
            var distinctRecipients = GetDistinctChatGroupConversationId(filterRecipients);

            var batches = DivideNotificationBatch(channelId, filterNotifications, distinctRecipients);
            foreach (var batch in batches)
            {
                await SendMultiMessageAsync(channelId, batch.Item1, batch.Item2, recipients);
            }
        }

        protected internal virtual async Task SendMultiMessageAsync(
            byte channelId,
            IEnumerable<NotificationMessage> messages,
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<NotificationRecipient> originRecipients)
        {
            try
            {
                var response = await chatbotService.SendMultiMessageAsync(channelId, messages);
                if (response.IsSuccessStatusCode && response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    if (response.NotificationMessageResult.Any())
                    {
                        await UpdateStatusAfterSendNotificationAsync(originRecipients, response.NotificationMessageResult);
                    }
                    else
                    {
                        await UpdateFailedStatusForBatchAsync(recipients, originRecipients, "Bot Service Error");
                    }
                }
                else
                {
                    await UpdateFailedStatusForBatchAsync(recipients, originRecipients, $"Status Code: {response.StatusCode}");
                    await logService.WriteMessageSentryErrorAsync($"Send Message fail - Status Code: {response.StatusCode}");
                }
            }
            catch (Exception exception)
            {
                await UpdateFailedStatusForBatchAsync(recipients, originRecipients, exception.Message);
                await logService.WriteExceptionSentryErrorAsync(exception);
            }
        }

        protected internal virtual async Task UpdateFailedStatusForBatchAsync(
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<NotificationRecipient> originRecipients,
            string errorMessage)
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

            await UpdateSentNotificationStatusAsync(updateNotiModels);
        }

        protected internal virtual async Task UpdateStatusAfterSendNotificationAsync(
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<SendMessageResult> responseMessages)
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
                        Retry = result.ErrorCode != 0 && !result.IsIgnoreRetry,
                        IsSuccess = result.ErrorCode == 0,
                        ErrMsg = result.ErrorCode == 0
                            ? string.Empty : GenerateErrorMessage(RecipientStatuses.BotServiceError, result.ErrorMessage)
                    };
                    updateNotiModels.Add(model);
                }
            }

            await UpdateSentNotificationStatusAsync(updateNotiModels);
        }

        protected internal virtual IEnumerable<(List<NotificationMessage>, List<NotificationRecipient>)> DivideNotificationBatch(
            byte channelId,
            IEnumerable<NotificationSendMessage> notifications,
            IEnumerable<NotificationRecipient> recipients)
        {
            var batches = new List<(List<NotificationMessage>, List<NotificationRecipient>)>();
            var batchRecipients = new List<List<NotificationRecipient>>();
            int itemEachBatch = appSettings.ItemEachBatchSendNotification;
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
                                    ChannelId = n.ChannelId,
                                    Message = n.Content
                                }));

                foreach (var item in newBatches)
                {
                    item.Recipients = new List<ConversationInfo>(batch.Where(r => r.NotificationId == item.NotificationId)
                        .Select(r => new ConversationInfo
                        {
                            BotConversationId = r.BotConversationId,
                            ServiceUrl = r.ServiceUrl,
                            ChannelId = r.ChannelName,
                            ConversationId = r.ChatGroupConversationId,
                            BotName = r.BotName
                        }).DistinctBy(r => r.ConversationId));
                }

                batches.Add((newBatches, batch));
            }

            batches.Last().Item1.Last().Recipients = channelId == (byte)ConversationChannel.Telegram
                ? AddMonitorRecipientForTelegram(batches.Last().Item1.Last().Recipients)
                : AddMonitorRecipient(batches.Last().Item1.Last().Recipients);

            return batches;
        }

        protected internal virtual IEnumerable<ConversationInfo> AddMonitorRecipient(IEnumerable<ConversationInfo> conversations)
        {
            if (monitorChatBot.IsMonitoring)
            {
                var conversation = new ConversationInfo
                {
                    ConversationId = monitorChatBot.ConversationId,
                    ChannelId = monitorChatBot.ChannelId,
                    ServiceUrl = monitorChatBot.ServiceUrl,
                    BotConversationId = monitorChatBot.BotConversationId,
                    BotName = monitorChatBot.BotName
                };

                conversations = conversations.Append(conversation);
            }

            return conversations;
        }

        protected internal virtual IEnumerable<ConversationInfo> AddMonitorRecipientForTelegram(IEnumerable<ConversationInfo> conversations)
        {
            if (monitorTelegramBot.IsMonitoring)
            {
                var conversation = new ConversationInfo
                {
                    ConversationId = monitorTelegramBot.ChatId.ToString(),
                    BotConversationId = monitorTelegramBot.BotId.ToString()
                };

                conversations = conversations.Append(conversation);
            }

            return conversations;
        }

        protected internal virtual async Task UpdateSentNotificationStatusAsync(IEnumerable<UpdateNotificationModel> messages)
        {
            if (messages.Any())
            {
                var command = new UpdateRecipientSendNotifiactionStatusCommand
                {
                    SentStatus = JsonConvert.SerializeObject(messages)
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

        private void PushSignalUpdateNotiStatus(string notificationIds)
        {
            Task.Run(() => pushSignalService.OnUpdateNotificationStatusAsync(notificationIds));
        }

        private void PushSignalNewErrNotification()
        {
            Task.Run(() => pushSignalService.OnHasNewErrNotificationAsync());
        }
    }
}