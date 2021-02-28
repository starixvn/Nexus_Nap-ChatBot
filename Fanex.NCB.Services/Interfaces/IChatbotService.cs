namespace Fanex.NCB.Services.Interfaces
{
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Shared.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IChatbotService
    {
        Task<HttpResponseMessage> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> messages);

        Task MonitorChatbot();

        Task NotifyToChatGroup(string message);

        IEnumerable<NotificationMessage> GenerateNotifificationMessage(IEnumerable<NotificationSendMessage> notifications,
                                                                        IEnumerable<NotificationRecipient> recipients);
    }
}