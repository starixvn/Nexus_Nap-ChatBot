namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;

    public interface IChatbotService
    {
        Task SendMessageAsync(string botName, string conversationId, string message);

        Task SendMessageToMultitpleAsync(IEnumerable<ProactiveMessage> proactiveMessages);
    }
}