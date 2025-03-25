using Fanex.NCB.BotService.EndPoint.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Fanex.NCB.BotService.EndPoint.Controllers.Teams
{
    [Route("api/teams/messages")]
    [ApiController]
    public class TeamsBotController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly IBotFrameworkHttpAdapter _adapter;

        public TeamsBotController(IBotFrameworkHttpAdapter adapter, [FromKeyedServices(ServiceName.TeamsBot)] IBot bot) 
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }

    }
}
