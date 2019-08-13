namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;
    using RestSharp;

    public interface IChatbotService
    {
        Task SendMessageAsync(string botName, string conversationId, string message);

        Task<IRestResponse> SendMessageToMultitpleAsync(IEnumerable<NotificationMessage> proactiveMessages);
    }
}