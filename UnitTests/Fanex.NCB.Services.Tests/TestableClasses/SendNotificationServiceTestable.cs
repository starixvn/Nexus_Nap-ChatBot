namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;

    public class SendNotificationServiceTestable : SendNotificationService
    {
        public SendNotificationServiceTestable(
               IChatbotService chatbotService,
               IBaseService baseService,
               IPushSignalService pushSignalService,
               ILogService logService,
               IOptionsSnapshot<AppSettings> appSettingOptions,
               IOptionsSnapshot<MonitorChatbot> monitorChatBotOptions,
               IOptionsSnapshot<MonitorTelegramBot> monitorTelegramBotOptions)
            : base(chatbotService, baseService, pushSignalService, logService, appSettingOptions, monitorChatBotOptions, monitorTelegramBotOptions)
        { }

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