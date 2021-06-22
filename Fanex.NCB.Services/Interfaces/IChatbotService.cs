namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Shared.Models;

    public interface IChatbotService
    {
        Task<NotificationMessageResponse> SendMultiMessageAsync(byte channelId, IEnumerable<NotificationMessage> messages);

        Task MonitorChatbotAsync();

        Task NotifyToChatGroupAsync(string message);

        IEnumerable<NotificationMessage> GenerateNotifificationMessageAsync(
            IEnumerable<NotificationSendMessage> notifications,
            IEnumerable<NotificationRecipient> recipients);

        Task MonitorTelegramBotAsync();
    }
}