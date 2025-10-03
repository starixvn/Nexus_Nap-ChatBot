namespace Fanex.NCB.BotService.EndPoint.Tests.Teams;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.BotService.Core.Enum;
using Fanex.NCB.BotService.Core.Services.Interfaces;
using Fanex.NCB.BotService.EndPoint.Controllers;
using Fanex.NCB.BotService.Core;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using FluentAssertions;

public class TeamsMessageControllerTests
{
    private readonly IFixture fixture = new Fixture();
    private AppSettings appSettings;
    private ICommonLogService commonLogService;
    private IMessageLogService messageLogService;
    private IConversationService conversationService;
    private IBotFrameworkService botService;

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var controller = GetController();

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeOfType<TeamsMessageController>();
    }

    [Fact]
    public async Task Send_WithEnabledTeamsChannel_SendsMessage()
    {
        // Arrange
        var controller = GetController();
        var conversationInfo = fixture.Create<ConversationInfo>();
        appSettings.EnableChannelIds = ((byte)ConversationChannel.Teams).ToString();

        // Act
        var result = await controller.Send(conversationInfo);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().Be(string.Empty);
        await botService.Received(1).SendMessageAsync(conversationInfo);
    }

    [Fact]
    public async Task Send_WithDisabledTeamsChannel_ReturnsErrorMessage()
    {
        // Arrange
        var controller = GetController();
        var conversationInfo = fixture.Create<ConversationInfo>();
        appSettings.EnableChannelIds = ((byte)ConversationChannel.Telegram).ToString();

        // Act
        var result = await controller.Send(conversationInfo);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().NotBe(string.Empty);
        await botService.DidNotReceive().SendMessageAsync(Arg.Any<ConversationInfo>());
    }

    [Fact]
    public async Task Send_WithException_ReturnsErrorMessage()
    {
        // Arrange
        var controller = GetController();
        var conversationInfo = fixture.Create<ConversationInfo>();
        var exceptionMessage = "Test exception";
        appSettings.EnableChannelIds = ((byte)ConversationChannel.Teams).ToString();

        botService.SendMessageAsync(conversationInfo)
            .Returns(Task.FromException(new Exception(exceptionMessage)));

        // Act
        var result = await controller.Send(conversationInfo);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task SendMulti_WithValidNotificationMessages_ReturnsResults()
    {
        // Arrange
        var controller = GetController();
        var notificationMessages = fixture.Create<List<NotificationMessage>>();
        var distinctNotifications = fixture.Create<List<NotificationMessage>>();
        var sendResults = fixture.Create<List<SendMessageResult>>();

        conversationService.GetDistinctNotificationConversation(notificationMessages)
            .Returns(distinctNotifications);

        // Act
        var result = await controller.SendMulti(notificationMessages);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeOfType<List<SendMessageResult>>();
    }

    [Fact]
    public async Task SendMulti_WithNullNotificationMessages_ReturnsEmptyResults()
    {
        // Arrange
        var controller = GetController();

        // Act
        var result = await controller.SendMulti(null);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var results = okResult.Value as List<SendMessageResult>;
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task SendMulti_WithEmptyNotificationMessages_ReturnsEmptyResults()
    {
        // Arrange
        var controller = GetController();
        var emptyList = new List<NotificationMessage>();

        // Act
        var result = await controller.SendMulti(emptyList);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var results = okResult.Value as List<SendMessageResult>;
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task SendIndirectMessageAsync_WithValidConversationInfo_ReturnsSuccessResult()
    {
        // Arrange
        var controller = GetController();
        var conversationInfo = fixture.Create<ConversationInfo>();

        // Act
        var result = await controller.SendIndirectMessageAsync(conversationInfo);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var sendResult = okResult.Value as SendMessageResult;
        sendResult.Should().NotBeNull();
        sendResult.ErrorCode.Should().Be(0);
        await botService.Received(1).SendMessageAsync(conversationInfo);
    }

    [Fact]
    public async Task SendIndirectMessageAsync_WithException_ReturnsErrorResult()
    {
        // Arrange
        var controller = GetController();
        var conversationInfo = fixture.Create<ConversationInfo>();
        var exceptionMessage = "Test exception";

        botService.SendMessageAsync(conversationInfo)
            .Returns(Task.FromException(new Exception(exceptionMessage)));

        // Act
        var result = await controller.SendIndirectMessageAsync(conversationInfo);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var sendResult = okResult.Value as SendMessageResult;
        sendResult.Should().NotBeNull();
        sendResult.ErrorCode.Should().Be(1);
        sendResult.ErrorMessage.Should().Be(exceptionMessage);
    }

    private TeamsMessageController GetController()
    {
        appSettings = new AppSettings
        {
            EnableChannelIds = ((byte)ConversationChannel.Teams).ToString()
        };
        commonLogService = Substitute.For<ICommonLogService>();
        messageLogService = Substitute.For<IMessageLogService>();
        conversationService = Substitute.For<IConversationService>();
        botService = Substitute.For<IBotFrameworkService>();

        var appSettingsOptions = Substitute.For<IOptions<AppSettings>>();
        appSettingsOptions.Value.Returns(appSettings);

        return new TeamsMessageController(
            appSettingsOptions,
            commonLogService,
            messageLogService,
            conversationService,
            botService);
    }
}
