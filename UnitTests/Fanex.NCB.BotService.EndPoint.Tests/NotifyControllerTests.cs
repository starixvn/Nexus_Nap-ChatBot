namespace Fanex.NCB.BotService.EndPoint.Tests;

using Controllers;
using Core;
using Core.Services;
using Core.Services.Interfaces;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class NotifyControllerTests
{
    private AppSettings appSettings;
    private IMessageLogService messageLogService;
    private IConversationService conversationService;
    private ICommonLogService commonLogService;
    private IBotFrameworkService skypeBotService;

    [Fact]
    public async Task Test_WhenCalled_ReturnAsExpectedResult()
    {
        // Arrange
        var controller = GetController();

        // Act
        var result = await controller.Test();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("ok", okResult.Value);

        await commonLogService.Received().SentryErrorAsync("test sentry");
    }

    #region Send

    [Fact]
    public async Task Send_DisableSkype_ReturnAsExpectedResult()
    {
        // Arrange
        var conversationInfo = new ConversationInfo();
        var controller = GetController("2");

        // Act
        var result = await controller.Send(conversationInfo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", okResult.Value);
    }

    [Fact]
    public async Task Send_EnableSkype_ReturnAsExpectedResult()
    {
        // Arrange
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "1",
            ConversationId = "1",
            Message = "message"
        };
        var controller = GetController();

        // Act
        var result = await controller.Send(conversationInfo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Empty((string)okResult.Value);

        await skypeBotService.Received().SendMessageAsync(conversationInfo);
    }

    #endregion Send

    [Fact]
    public async Task SendMulti_NotificationListIsNull_ReturnObjectResultWithEmptyList()
    {
        var controller = GetController();

        //Actual
        var actual = await controller.SendMulti(null);

        //Assert
        Assert.IsType<OkObjectResult>(actual);
        Assert.Empty((List<SendMessageResult>)actual.Value);
    }

    [Fact]
    public async Task SendMulti_NotificationListIsEmpty_ReturnObjectResultWithEmptyList()
    {
        var controller = GetController();
        var notificationMessage = new List<NotificationMessage>
            { new() { NotificationId = 1, Recipients = new List<ConversationInfo>() } };

        //Actual
        var actual = await controller.SendMulti(notificationMessage);

        //Assert
        Assert.IsType<OkObjectResult>(actual);
        Assert.Empty((List<SendMessageResult>)actual.Value);
    }

    [Fact]
    public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithSuccessList()
    {
        var controller = GetController();

        var notificationMessage = new List<NotificationMessage>
        {
            new()
            {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                {
                    new()
                    {
                        ConversationId = "ConversationId"
                    }
                }
            }
        };
        conversationService.GetDistinctNotificationConversation(Arg.Any<List<NotificationMessage>>())
            .Returns(notificationMessage);
        skypeBotService.SendMessageWithRetryAsync(Arg.Any<ConversationInfo>()).Returns(new SendMessageResult
        { NotificationId = 1, ConversationId = "ConversationId" });
        //Actual
        var actual = await controller.SendMulti(notificationMessage);

        //Assert
        Assert.IsType<OkObjectResult>(actual);
        Assert.Single((List<SendMessageResult>)actual.Value);
        Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
        Assert.Equal("ConversationId", ((List<SendMessageResult>)actual.Value)[0].ConversationId);
        Assert.Equal(0, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
        Assert.Null(((List<SendMessageResult>)actual.Value)[0].ErrorMessage);
    }

    [Fact]
    public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithErrorList()
    {
        var controller = GetController();

        var notificationMessage = new List<NotificationMessage>
        {
            new()
            {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                {
                    new()
                }
            }
        };
        conversationService.GetDistinctNotificationConversation(Arg.Any<List<NotificationMessage>>())
            .Returns(notificationMessage);
        skypeBotService.SendMessageWithRetryAsync(Arg.Any<ConversationInfo>())
            .Returns(new SendMessageResult
            { NotificationId = 1, ConversationId = "ConversationId", ErrorCode = 1, ErrorMessage = "error msg" });

        //Actual
        var actual = await controller.SendMulti(notificationMessage);

        //Assert
        Assert.IsType<OkObjectResult>(actual);
        Assert.Single((List<SendMessageResult>)actual.Value);
        Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
        Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
        Assert.Equal("error msg", ((List<SendMessageResult>)actual.Value)[0].ErrorMessage);
    }

    private NotifyController GetController(string enableChannelIds = "1,2")
    {
        appSettings = new AppSettings { EnableChannelIds = enableChannelIds };
        var appSettingOptions = Substitute.For<IOptions<AppSettings>>();
        appSettingOptions.Value.Returns(appSettings);

        commonLogService = Substitute.For<ICommonLogService>();
        messageLogService = Substitute.For<IMessageLogService>();
        conversationService = Substitute.For<IConversationService>();
        skypeBotService = Substitute.For<IBotFrameworkService>();
        return new NotifyController(appSettingOptions, commonLogService, messageLogService, conversationService, skypeBotService);
    }
}