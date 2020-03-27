namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Shared.Models;
    using RestSharp;

    public interface IChatbotService
    {
        Task SendMessageAsync(string botName, string conversationId, string message);

        Task<IRestResponse> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> messages);

        Task MonitorChatbot();

        Task NotifyToChatGroup(string message);

        IEnumerable<NotificationMessage> GenerateNotifificationMessage(IEnumerable<NotificationSendMessage> notifications,
                                                                        IEnumerable<NotificationRecipient> recipients);
    }
}