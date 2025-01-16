namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Configurations;
    using Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Microsoft.Extensions.Options;

    public class SendNotificationServiceTestable : SendNotificationService
    {
        public SendNotificationServiceTestable(
               IChatbotService chatbotService,
               IBaseService baseService,
               IPushSignalRService pushSignalService,
               ILogService logService,
               IOptionsSnapshot<AppSettings> appSettingOptions,
               IOptionsSnapshot<MonitorChatbot> monitorChatBotOptions,
               IOptionsSnapshot<MonitorTelegramBot> monitorTelegramBotOptions)
            : base(chatbotService, baseService, pushSignalService, logService, appSettingOptions, monitorChatBotOptions, monitorTelegramBotOptions)
        { }

        public async Task Public_UpdateFailedRecipientStatusAsync(IEnumerable<NotificationRecipient> recipients, bool isRetry = false)
        {
            await base.UpdateFailedRecipientStatusAsync(recipients, isRetry);
        }

        public async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> Public_GetNotificationPlannedToSendAsync()
        {
            return await base.GetNotificationPlannedToSendAsync();
        }

        public async Task Public_SendMultiMessageAsync(
            byte channelId,
            IEnumerable<NotificationMessage> messages,
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<NotificationRecipient> originRecipients)
        {
            await base.SendMultiMessageAsync(channelId, messages, recipients, originRecipients);
        }

        public async Task Public_UpdateFailedStatusForBatchAsync(
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<NotificationRecipient> originRecipients,
            string errorMessage)
        {
            await base.UpdateFailedStatusForBatchAsync(recipients, originRecipients, errorMessage);
        }

        public async Task Public_UpdateStatusAfterSendNotificationAsync(
            IEnumerable<NotificationRecipient> recipients,
            IEnumerable<SendMessageResult> responseMessages)
        {
            await base.UpdateStatusAfterSendNotificationAsync(recipients, responseMessages);
        }

        public IEnumerable<(List<NotificationMessage>, List<NotificationRecipient>)> Public_DivideNotificationBatch(
            byte channelId,
            IEnumerable<NotificationSendMessage> notifications,
            IEnumerable<NotificationRecipient> recipients)
        {
            return base.DivideNotificationBatch(channelId, notifications, recipients);
        }

        public IEnumerable<ConversationInfo> Public_AddMonitorRecipient(IEnumerable<ConversationInfo> conversations)
        {
            return base.AddMonitorRecipient(conversations);
        }

        public IEnumerable<ConversationInfo> Public_AddMonitorRecipientForTelegram(IEnumerable<ConversationInfo> conversations)
        {
            return base.AddMonitorRecipientForTelegram(conversations);
        }

        public async Task Public_UpdateSentNotificationStatusAsync(IEnumerable<UpdateNotificationModel> messages)
        {
            await base.UpdateSentNotificationStatusAsync(messages);
        }
    }
}
