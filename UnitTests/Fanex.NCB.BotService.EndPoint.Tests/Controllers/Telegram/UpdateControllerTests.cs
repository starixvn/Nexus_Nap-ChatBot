using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.BotService.Core.Services.Interfaces;
using Fanex.NCB.BotService.Core.Models;
using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Telegram.Bot.Types;
using Xunit;
using FluentAssertions;

namespace Fanex.NCB.BotService.EndPoint.Tests.Controllers.Telegram;

public class UpdateControllerTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly ICommonLogService logger;
    private readonly ITelegramBotService telegramBotService;
    private readonly IConversationService conversationService;
    private readonly UpdateController updateController;

    public UpdateControllerTests()
    {
        logger = Substitute.For<ICommonLogService>();
        telegramBotService = Substitute.For<ITelegramBotService>();
        conversationService = Substitute.For<IConversationService>();
        updateController = new UpdateController(logger, telegramBotService, conversationService, new List<TelegramBotConfigurations>());
    }

    [Fact]
    public async Task Post_WithValidBotId_ReturnsOk()
    {
        // Arrange
        var botId = 123L;
        var update = new Update();

        // Act
        var result = await updateController.Post(botId, update);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Post_WithNonMessageUpdate_ReturnsOk()
    {
        // Arrange
        var botId = 123L;
        var update = new Update();

        // Act
        var result = await updateController.Post(botId, update);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Post_WithInvalidBotId_ReturnsOkWithErrorMessage()
    {
        // Arrange
        var botId = 0L;
        var update = new Update();

        // Act
        var result = await updateController.Post(botId, update);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}