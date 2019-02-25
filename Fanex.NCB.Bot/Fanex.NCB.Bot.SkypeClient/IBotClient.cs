using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.SkypeClient
{
    public interface IBotClient
    {
        Task<HttpStatusCode> SendAsync(string message, string conversationId);
        Task<HttpStatusCode> SendAsync(IMessageActivity messageActivity);
        Task<HttpStatusCode> ReplyAsync(IMessageActivity activity, string message);

    }
}
