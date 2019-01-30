using System.Diagnostics;
using System.Threading.Tasks;
using Fanex.NCB.Bot.SkypeClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.Main.Controllers
{
    [Authorize]
    public class SkypeBotController : Controller
    {
        private readonly IBotClient _botClient;
        public SkypeBotController(IBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task<string> Index(string message, string conversationid)
        {

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(conversationid))
            {
                await _botClient.SendAsync(message, conversationid);
            }
            return "OK";
        }

    }
}
