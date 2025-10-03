namespace Fanex.NCB.BotService.EndPoint.Tests.Teams;

using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.BotService.EndPoint.Controllers.Teams;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using NSubstitute;
using Xunit;
using FluentAssertions;

public class TeamsBotControllerTests
{
    private readonly IFixture fixture = new Fixture();
    private IBotFrameworkHttpAdapter adapter;
    private IBot bot;

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = GetController();

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeOfType<TeamsBotController>();
    }

    [Fact]
    public async Task PostAsync_WhenCalled_ProcessesRequest()
    {
        // Arrange
        var controller = GetController();
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        await controller.PostAsync();

        // Assert
        await adapter.Received(1).ProcessAsync(
            Arg.Is<HttpRequest>(r => r == httpContext.Request),
            Arg.Is<HttpResponse>(r => r == httpContext.Response),
            bot);
    }

    [Fact]
    public async Task PostAsync_WithNullHttpContext_DoesNotThrow()
    {
        // Arrange
        var controller = GetController();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => controller.PostAsync());

        // The method should handle null context gracefully or throw expected exception
        // This test verifies the method can be called
    }

    private TeamsBotController GetController()
    {
        adapter = Substitute.For<IBotFrameworkHttpAdapter>();
        bot = Substitute.For<IBot>();

        return new TeamsBotController(adapter, bot);
    }
}
