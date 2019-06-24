using Microsoft.Bot.Schema;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.NCB.Bot.SkypeClient
{
    public interface IBotClient
    {
        Task<HttpStatusCode> SendAsync(string message, string conversationId);

        Task<HttpStatusCode> SendAsync(IMessageActivity messageActivity);
    }
}