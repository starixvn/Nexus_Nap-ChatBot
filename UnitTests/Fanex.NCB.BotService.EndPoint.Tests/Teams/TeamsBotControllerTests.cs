using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using Fanex.NCB.BotService.EndPoint.Controllers.Teams;

namespace Fanex.NCB.BotService.EndPoint.Tests.Teams
{
    public class TeamsBotControllerTests
    {
        private IBotFrameworkHttpAdapter adapter;
        private IBot bot;

        [Fact]
        public async Task PostAsync_WhenCalled_ExecuteAdapter()
        {
            var controller = GetController();
            //Act
            await controller.PostAsync();

            await adapter.Received().ProcessAsync(Arg.Any<HttpRequest>(), Arg.Any<HttpResponse>(), bot);
        }

        private TeamsBotController GetController()
        {
            adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            bot = Substitute.For<IBot>();
            return new TeamsBotController(adapter, bot);
        }
    }
}
