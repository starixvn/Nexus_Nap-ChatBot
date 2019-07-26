using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.NCB.Base.Models;
using Fanex.NCB.Bot.SkypeClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fanex.NCB.Main.Controllers
{
    [Authorize]
    public class SkypeBotController : Controller
    {
        private readonly IList<SkypeBotSetting> _options;

        public SkypeBotController(IOptions<List<SkypeBotSetting>> options)
        {
            _options = options.Value;
        }

        public async Task<string> Index(string message, string conversationid)
        {
            var spartaBot = _options.FirstOrDefault(i => i.Name == "SpartaBot");
            var botClient = new BotClient(spartaBot.MicrosoftAppId, spartaBot.MicrosoftAppPassword);
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(conversationid))
            {
                await botClient.SendAsync(message, conversationid).ConfigureAwait(false);
            }

            return "OK";
        }
    }
}