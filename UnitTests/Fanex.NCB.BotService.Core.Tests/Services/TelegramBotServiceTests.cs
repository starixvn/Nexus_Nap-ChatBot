namespace Fanex.NCB.BotService.Core.Tests.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.BotService.Core;
using Fanex.NCB.BotService.Core.Criteria;
using Fanex.NCB.BotService.Core.Models;
using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.BotService.Core.Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Telegram.Bot;
using Xunit;

public class TelegramBotServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppSettings appSettings;
    private readonly IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings;
    private readonly IOptions<ProxySettings> proxySettings;
    private readonly List<TelegramBotConfigurations> botConfigurations;
    private readonly ICommonLogService logService;
    private readonly IConversationService conversationService;

    public TelegramBotServiceTests()
    {
        httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient());
        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());

        appSettings = new AppSettings
        {
            Domain = "https://test.domain.com"
        };

        sendIndirectMessageSettings = Options.Create(new SendIndirectMessageSettings
        {
            IsOnlySendIndirect = false,
            IsEnabled = true,
            AppCredentialId = "testId",
            AppCredentialPassword = "testPassword",
            AuthenticateUrl = "https://auth.test.com/token",
            TelegramUrl = "https://telegram.test.com/send"
        });

        proxySettings = Options.Create(new ProxySettings
        {
            IsUseProxy = false,
            TimeOut = 30
        });

        botConfigurations = new List<TelegramBotConfigurations>
        {
            new TelegramBotConfigurations
            {
                BotId = 123,
                Token = "test-token",
                Username = "testbot",
                BotClient = Substitute.For<ITelegramBotClient>()
            }
        };

        logService = Substitute.For<ICommonLogService>();
        conversationService = Substitute.For<IConversationService>();
    }

    private TelegramBotService GetService()
    {
        return new TelegramBotService(
            httpClientFactory,
            appSettings,
            sendIndirectMessageSettings,
            proxySettings,
            botConfigurations,
            logService,
            conversationService);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = GetService();

        // Assert
        service.Should().NotBeNull();
        service.Should().BeOfType<TelegramBotService>();
    }

    [Fact]
    public void Constructor_WithProxyEnabled_CreatesInstance()
    {
        // Arrange
        var proxySettingsWithProxy = Options.Create(new ProxySettings
        {
            IsUseProxy = true,
            TimeOut = 60
        });

        // Act
        var service = new TelegramBotService(
            httpClientFactory,
            appSettings,
            sendIndirectMessageSettings,
            proxySettingsWithProxy,
            botConfigurations,
            logService,
            conversationService);

        // Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region Basic Functionality Tests

    [Fact]
    public async Task SendTextMessageAsync_WithValidConversationInfo_ReturnsResult()
    {
        // Arrange
        var service = GetService();
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            Message = "Test message",
            NotificationId = 1
        };

        // Act
        var result = await service.SendTextMessageAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
    }

    [Fact]
    public async Task SendMessageWithImagesAsync_WithValidConversationInfo_ReturnsResult()
    {
        // Arrange
        var service = GetService();
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            Message = "Test message",
            NotificationId = 1,
            Attachments = new List<string> { "image1.jpg" }
        };

        // Act
        var result = await service.SendMessageWithImagesAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
    }

    [Fact]
    public async Task ResendImageOnlyAsync_WithFailedImages_ReturnsResult()
    {
        // Arrange
        var service = GetService();
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            NotificationId = 1,
            FailedImages = new List<string> { "image1.jpg" }
        };

        // Act
        var result = await service.ResendImageOnlyAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
    }

    [Fact]
    public async Task SendImageAsync_WithInvalidBotId_ThrowsException()
    {
        // Arrange
        var service = GetService();
        var invalidBotId = 999L;
        var chatId = 456L;
        var urlImage = "https://example.com/image.jpg";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.SendImageAsync(invalidBotId, chatId, urlImage));

        exception.Message.Should().Contain("999 does not existed on config file");
    }

    #endregion

    #region Additional Basic Tests

    [Fact]
    public void Constructor_WithValidParameters_InitializesBotClients()
    {
        // Arrange & Act
        var service = GetService();

        // Assert
        service.Should().NotBeNull();
        botConfigurations[0].BotClient.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBotInfoAsync_WithInvalidBotId_ThrowsNullReferenceException()
    {
        // Arrange
        var service = GetService();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetBotInfoAsync(999));
    }

    [Fact]
    public async Task SendTextMessageAsync_WithInvalidBotId_ThrowsNullReferenceException()
    {
        // Arrange
        var service = GetService();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => service.SendTextMessageAsync(999, 456, "Test message"));
    }

    [Fact]
    public async Task SendTextMessageAsync_WithOnlySendIndirectEnabled_ReturnsResult()
    {
        // Arrange
        var sendIndirectSettings = Options.Create(new SendIndirectMessageSettings
        {
            IsOnlySendIndirect = true,
            IsEnabled = true,
            AppCredentialId = "testId",
            AppCredentialPassword = "testPassword",
            AuthenticateUrl = "https://auth.test.com/token",
            TelegramUrl = "https://telegram.test.com/send"
        });

        var service = new TelegramBotService(
            httpClientFactory,
            appSettings,
            sendIndirectSettings,
            proxySettings,
            botConfigurations,
            logService,
            conversationService);

        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            Message = "Test message",
            NotificationId = 1
        };

        // Act
        var result = await service.SendTextMessageAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
    }

    [Fact]
    public async Task SendMessageWithImagesAsync_WithNoAttachments_OnlySendsTextMessage()
    {
        // Arrange
        var service = GetService();
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            Message = "Test message",
            NotificationId = 1,
            Attachments = null
        };

        // Act
        var result = await service.SendMessageWithImagesAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
        await conversationService.Received(1).AddMessageLog(Arg.Any<AddMessageLogCommand>());
    }

    [Fact]
    public async Task SendMessageWithImagesAsync_WithEmptyAttachments_OnlySendsTextMessage()
    {
        // Arrange
        var service = GetService();
        var conversationInfo = new ConversationInfo
        {
            BotConversationId = "123",
            ConversationId = "456",
            Message = "Test message",
            NotificationId = 1,
            Attachments = new List<string>()
        };

        // Act
        var result = await service.SendMessageWithImagesAsync(conversationInfo);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationInfo.ConversationId);
        result.NotificationId.Should().Be(conversationInfo.NotificationId);
        await conversationService.Received(1).AddMessageLog(Arg.Any<AddMessageLogCommand>());
    }

    #endregion
}