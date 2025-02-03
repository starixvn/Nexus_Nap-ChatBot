namespace Fanex.NCB.BotService.Core.Tests.Services;

using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
using Xunit;

[Obsolete]
public class SkypeBotServiceTests
{
    private IHttpClientFactory httpClientFactory;
    private IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings;
    private IOptions<ProxySettings> proxySettings;
        private ICommonLogService commonLogService;
    private IAdapterIntegration adapter;
    private BotConfiguration botConfig;

    private readonly ConversationInfo FakeConversationInfo = new()
    {
        BotConversationId = "BotId",
        BotName = "BotName",
        ConversationId = "SkypeConversationId"
    };

    [Fact]
    public async Task HttpClientFactory_CallCreateClientWithoutName_WhenNotUseProxy()
    {
        var service = GetService(false, true);
        await service.SendMessageAsync(FakeConversationInfo);
        httpClientFactory.Received().CreateClient();
    }

    [Fact]
    public async Task HttpClientFactory_CallCreateClientWithExpectedtName_WhenUseProxy()
    {
        var service = GetService(true, true);
        await service.SendMessageAsync(FakeConversationInfo);
        httpClientFactory.Received().CreateClient(Arg.Is(NamedHttpClients.ProxiedClient));
    }

    [Fact]
    public async Task SendMessageWithRetryAsync_WhenNoException_ReturnExpectedResult()
    {
        var service = GetService(true, true);
        var actual = await service.SendMessageWithRetryAsync(FakeConversationInfo);
        Assert.Equal(FakeConversationInfo.ConversationId, actual.ConversationId);
        Assert.Equal(FakeConversationInfo.NotificationId, actual.NotificationId);
    }

    [Fact]
    public async Task SendMessageWithRetryAsync_WhenExceptionAndEnableSendIndirect_ReturnExpectedResult()
    {
        var service = GetService(true, true);
        adapter.ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(),
            Arg.Any<BotCallbackHandler>()).Throws(new Exception("Error Message"));
        var actual = await service.SendMessageWithRetryAsync(FakeConversationInfo);
            await commonLogService.Received().SentryErrorAsync(Arg.Any<string>());
        Assert.Equal(1, actual.ErrorCode);
        Assert.Equal("Error Message", actual.ErrorMessage);
        Assert.Equal(FakeConversationInfo.ConversationId, actual.ConversationId);
        Assert.Equal(FakeConversationInfo.NotificationId, actual.NotificationId);
    }

    private SkypeBotService GetService(bool isUserProxy, bool isEnableSendIndirect)
    {
        httpClientFactory = Substitute.For<IHttpClientFactory>();

        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());

        sendIndirectMessageSettings = Substitute.For<IOptions<SendIndirectMessageSettings>>();
        proxySettings = Substitute.For<IOptions<ProxySettings>>();
        sendIndirectMessageSettings.Value.Returns(
            new SendIndirectMessageSettings
            {
                IsEnabled = isEnableSendIndirect,
                AuthenticateUrl = "Test.com",
                AppCredentialId = "appId",
                AppCredentialPassword = "AppPwd",
                SkypeUrl = "skype.com"
            });

        proxySettings.Value.Returns(new ProxySettings
            commonLogService = Substitute.For<ICommonLogService>();
        adapter = Substitute.For<IAdapterIntegration>();
        botConfig = Substitute.For<BotConfiguration>();
        botConfig.Services = new List<ConnectedService> { new EndpointService { Name = FakeConversationInfo.BotName } };
            return new SkypeBotService(httpClientFactory, sendIndirectMessageSettings, proxySettings, commonLogService, adapter, botConfig);
            adapter, botConfig);
    }
}