using Fanex.NCB.Shared.Models;
using System.Threading.Tasks;

namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    public interface IBotFrameworkService
    {
        Task SendMessageAsync(ConversationInfo conversationInfo);

        Task<SendMessageResult> SendMessageWithRetryAsync(ConversationInfo conversationInfo);
    }
}
