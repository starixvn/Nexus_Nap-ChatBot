namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using System.Threading.Tasks;

    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly IBot bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            this.adapter = adapter;
            this.bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            await adapter.ProcessAsync(Request, Response, bot);
        }
    }
}