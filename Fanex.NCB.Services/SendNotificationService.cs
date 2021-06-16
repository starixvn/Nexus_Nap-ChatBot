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
        private readonly IOptions<AppSettings> appSettings;
        private readonly IOptions<MonitorChatbot> monitorOptions;
        private readonly ILogService logService;
        private readonly MonitorTelegramBot monitorTelegramBot;

        public SendNotificationService(IChatbotService chatbotService,
            IBaseService baseService,
            IPushSignalService pushSignalService,
            IOptions<AppSettings> appSettings,
            IOptions<MonitorChatbot> monitorOptions,
            ILogService logService,
            IOptions<MonitorTelegramBot> monitorTelegramBot)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.appSettings = appSettings;
            this.monitorOptions = monitorOptions;
            this.logService = logService;
            this.monitorTelegramBot = monitorTelegramBot.Value;
        }

        public async Task ScanAndSendNotification()
        {
            var (notifications, recipients) = await GetNotificationPlannedToSend();
            if (notifications.Any())
            {
                await UpdateFailedRecipientStatus(recipients.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));
                var readyToSendRecipients = recipients.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend));

                await SendNotificationToRecipients(notifications, readyToSendRecipients);
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

        private async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationPlannedToSend()
        {
            var criteria = new GetNotificationsToSendCriteria();
            var notifications = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);
            if (!string.IsNullOrWhiteSpace(criteria.UpdatedNotificationIds))
            {
                await PushSignalUpdateNotiStatus(criteria.UpdatedNotificationIds);
            }
            return notifications;
        }

        private static IEnumerable<NotificationRecipient> GetDistinctChatGroupConversationId(IEnumerable<NotificationRecipient> recipients)
        {
            var cloneRecipients = new List<NotificationRecipient>(recipients);
            return cloneRecipients.GroupBy(g => new { g.NotificationId, g.ChatGroupConversationId })
                .Select(group => group.FirstOrDefault()).OrderBy(r => r.NotificationId);
        }

        private async Task SendNotificationToRecipients(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            await Task.WhenAll(
                SendMessageByChannelAsync((byte)ConversationChannel.Skype, notifications, recipients),
                SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notifications, recipients)
            );
        }

        private async Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var filterNotifications = notifications.Where(x => x.ChannelId == channelId).ToList();
            var filterRecipients = recipients.Where(x => filterNotifications.Any(y => y.NotificationId == x.NotificationId)).ToList();
            var distinctRecipients = GetDistinctChatGroupConversationId(filterRecipients);

            var batches = DivideNotificationBatch(channelId, filterNotifications, distinctRecipients);
            foreach (var batch in batches)
            {
                await SendMultiMessageAsync(channelId, batch.Item1, batch.Item2, recipients);
            }
        }

        private async Task SendMultiMessageAsync(
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
                        await UpdateStatusAfterSendNotification(originRecipients, response.NotificationMessageResult);
                    }
                    else
                    {
                        await UpdateFailedStatusForBatch(recipients, originRecipients, "Bot Service Error");
                    }
                }
                else
                {
                    await UpdateFailedStatusForBatch(recipients, originRecipients, $"Status Code: { response.StatusCode}");
                    await logService.WriteMessageSentryErrorAsync($"Send Message fail - Status Code: {response.StatusCode}");
                }
            }
            catch (Exception exception)
            {
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
                        Retry = result.ErrorCode != 0 && !result.IsIgnoreRetry,
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
            DivideNotificationBatch(byte channelId, IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
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
                batches.Add(new ValueTuple<List<NotificationMessage>, List<NotificationRecipient>>(newBatches, batch));
            }

            switch (channelId)
            {
                case (byte)ConversationChannel.Skype:
                    batches.Last().Item1.Last().Recipients = AddMonitorRecipient(batches.Last().Item1.Last().Recipients);
                    break;

                case (byte)ConversationChannel.Telegram:
                    batches.Last().Item1.Last().Recipients = AddMonitorRecipientForTelegram(batches.Last().Item1.Last().Recipients);
                    break;

                default:
                    // do nothing
                    break;
            }

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

        private IEnumerable<ConversationInfo> AddMonitorRecipientForTelegram(IEnumerable<ConversationInfo> conversations)
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
                    await PushSignalUpdateNotiStatus(command.UpdatedNotificationIds);
                }

                if (command.IsHaveErrNotification)
                {
                    await PushSignalNewErrNotification();
                }
            }
        }

        public async Task SendNotificationImmediately(int notifiactionId)
        {
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = notifiactionId
            };
            var (notifications, recipients) = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria);

            await UpdateFailedRecipientStatus(recipients.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)
                    && !r.RecipientStatus.Equals(RecipientStatuses.BotServiceError)));
            await SendNotificationToRecipients(notifications, recipients.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));
        }

        private async Task PushSignalUpdateNotiStatus(string notificationIds)
        {
            await pushSignalService.OnUpdateNotificationStatus(notificationIds);
        }

        private async Task PushSignalNewErrNotification()
        {
            await pushSignalService.OnHasNewErrNotifications();
        }
    }
}