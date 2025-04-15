using AutoFixture;
using Fanex.NCB.BotService.Core.Models;
using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.BotService.Core.Services.Interfaces;
using Fanex.NCB.Shared.Extensions;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Fanex.NCB.Resources;
using Microsoft.Bot.Builder;
using Telegram.Bot.Types;

namespace Fanex.NCB.BotService.Core.Tests.Services
{
    public class TelegramBotServiceTests
    {
        private HttpClient _httpClient;
        private AppSettings _appSettings;
        private SendIndirectMessageSettings _sendIndirectMessageSettings;
        private List<TelegramBotConfigurations> _botConfigurations;
        private ICommonLogService _commonLogService;
        private IConversationService _conversationService;
        private MockHttpMessageHandler _handler;
        private ProxySettings _proxySettings;
        private IHttpClientFactory _httpClientFactory;

        private Fixture _fixture;

        private IDictionary<string, string> _teleErrorMsgMapping = new Dictionary<string, string>
        {
            { "Message is too long", Resources.Resources.MessageIsTooLong },
            { "bot was kicked from the supergroup chat",  Resources.Resources.GroupChatDeleted },
            { "bot was kicked from the group chat", Resources.Resources.GroupChatDeleted },
            { "chat not found", Resources.Resources.GroupChatDeleted }
        };

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize<TelegramBotConfigurations>(x => x.Without(y => y.BotClient));

            _handler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_handler);
            _appSettings = _fixture.Create<AppSettings>();
            _sendIndirectMessageSettings = _fixture.Create<SendIndirectMessageSettings>();
            _botConfigurations = _fixture.CreateMany<TelegramBotConfigurations>(1).ToList();
            _proxySettings = _fixture.Create<ProxySettings>();
            _commonLogService = Substitute.For<ICommonLogService>();
            _conversationService = Substitute.For<IConversationService>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            _handler.Dispose();
            _appSettings = null;
            _sendIndirectMessageSettings = null;
            _botConfigurations = null;
            _commonLogService = null;
            _conversationService = null;
            _handler = null;
            _httpClient = null;
            _proxySettings = null;
            _httpClientFactory = null;
        }

        [Test]
        public void Constructor_WhenCallNoProxy_CreateNormalHttpClient()
        {
            // Arrange
            _proxySettings.IsUseProxy = false;

            // Act
            CreateService();

            // Assert
            _httpClientFactory.Received(1).CreateClient();
            _httpClientFactory.DidNotReceive().CreateClient(Arg.Is<string>(x => x == NamedHttpClients.ProxiedClient));
        }

        [Test]
        public void Constructor_WhenCallWithProxy_CreateProxiedHttpClient()
        {
            // Arrange
            _proxySettings.IsUseProxy = true;

            // Act
            CreateService();

            // Assert
            _httpClientFactory.Received(1).CreateClient(NamedHttpClients.ProxiedClient);
            _httpClientFactory.DidNotReceive().CreateClient();
        }

        [Test]
        public async Task SetWebHookAsync_WhenCall_TelegramCallRelevantMethod()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var url = $"{_appSettings.Domain}/api/telegram/webhook/{bot.BotId}/update";

            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;

            // Act
            await service.SetWebhookAsync();

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Any<DeleteWebhookRequest>());
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Is<SetWebhookRequest>(_ => _.Url == url));
        }

        [Test]
        public async Task GetBotInfoAsync_WhenCall_TelegramCallRelevantMethod()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;

            // Act
            await service.GetBotInfoAsync(bot.BotId);

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Any<GetMeRequest>());
        }

        [Test]
        public async Task SendTextMessageAsync_WhenCall_ReturnCorrectText()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";

            // Act
            await service.SendTextMessageAsync(bot.BotId, chatId, message);

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Is<SendMessageRequest>(_ => _.ChatId.Identifier == chatId && _.Text == message));

        }

        [Test]
        public async Task SendTextMessageAsync_WhenNotSendOnlyIndirect_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            _sendIndirectMessageSettings.IsOnlySendIndirect = false;

            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;

            // Act
            var result = await service.SendTextMessageAsync(conversation);

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Is<SendMessageRequest>(_ => _.ChatId.Identifier == chatId && _.Text == message.ToMarkdownV2()));
            Assert.That(conversation.ConversationId, Is.EqualTo(result.ConversationId));
            Assert.That(conversation.NotificationId, Is.EqualTo(result.NotificationId));
        }

        [Test]
        public async Task SendTextMessageAsync_WhenSendOnlyIndirect_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            _sendIndirectMessageSettings.IsOnlySendIndirect = true;
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;
            _sendIndirectMessageSettings.TelegramUrl = "https://example.com/send";

            SetUpGetToken(new ChatbotToken { AccessToken = "access_token" });
            _handler.When(HttpMethod.Post, _sendIndirectMessageSettings.TelegramUrl)
                .Respond("application/json", JsonConvert.SerializeObject(new SendMessageResult()));

            // Act
            var result = await service.SendTextMessageAsync(conversation);

            // Assert
            await mockTeleClient.DidNotReceive().MakeRequestAsync(request: Arg.Is<SendMessageRequest>(_ => _.ChatId.Identifier == chatId && _.Text == message.ToMarkdownV2()));
            Assert.That(conversation.ConversationId, Is.EqualTo(result.ConversationId));
            Assert.That(conversation.NotificationId, Is.EqualTo(result.NotificationId));
            Assert.That(result.ErrorCode, Is.EqualTo(0));
        }

        [Test]
        public async Task SendTextMessageAsync_WhenSendOnlyIndirectAndGetTokenFailed_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            _sendIndirectMessageSettings.IsOnlySendIndirect = true;
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;
            SetUpGetToken(new ChatbotToken { AccessToken = string.Empty });

            // Act
            var result = await service.SendTextMessageAsync(conversation);

            // Assert
            await _commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
            Assert.That(conversation.ConversationId, Is.EqualTo(result.ConversationId));
            Assert.That(conversation.NotificationId, Is.EqualTo(result.NotificationId));
            Assert.That(result.ErrorCode, Is.EqualTo(1));
        }

        [TestCase("Message is too long")]
        [TestCase("bot was kicked from the supergroup chat")]
        [TestCase("bot was kicked from the group chat")]
        [TestCase("chat not found")]
        [TestCase("Exception not belong to any case")]
        public async Task SendTextMessageAsync_WhenNotOnlySendIndirect_AndTelegramBotClientSendFailed_AndNotEnableSendIndirect_ReturnCorrectResult(string errMsg)
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            _sendIndirectMessageSettings.IsOnlySendIndirect = false;

            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;

            mockTeleClient.When(x => x.MakeRequestAsync(request: Arg.Any<SendMessageRequest>())).Throw(new Exception(errMsg));
            _sendIndirectMessageSettings.IsEnabled = false;

            var isIgnoreRetry = _teleErrorMsgMapping.TryGetValue(errMsg, out var resultMsg);
            if (!isIgnoreRetry)
            {
                resultMsg = errMsg;
            }

            // Act
            var result = await service.SendTextMessageAsync(conversation);

            // Assert
            Assert.That(conversation.ConversationId, Is.EqualTo(result.ConversationId));
            Assert.That(conversation.NotificationId, Is.EqualTo(result.NotificationId));
            Assert.That(result.ErrorCode, Is.EqualTo(1));
            Assert.That(result.ErrorMessage, Is.EqualTo(resultMsg));
            Assert.That(result.IsIgnoreRetry, Is.EqualTo(isIgnoreRetry));
        }

        [Test]
        public async Task SendTextMessageAsync_WhenNotOnlySendIndirect_AndTelegramBotClientSendFailed_AndEnableSendIndirect_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            _sendIndirectMessageSettings.IsOnlySendIndirect = false;
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;
            mockTeleClient.When(x => x.MakeRequestAsync(request: Arg.Any<SendMessageRequest>())).Throw(new Exception("chat not found"));

            _sendIndirectMessageSettings.IsEnabled = true;
            _sendIndirectMessageSettings.TelegramUrl = "https://example.com/send";

            SetUpGetToken(new ChatbotToken { AccessToken = "access_token" });
            _handler.When(HttpMethod.Post, _sendIndirectMessageSettings.TelegramUrl)
                .Respond("application/json", JsonConvert.SerializeObject(new SendMessageResult()));

            // Act
            var result = await service.SendTextMessageAsync(conversation);

            // Assert
            Assert.That(conversation.ConversationId, Is.EqualTo(result.ConversationId));
            Assert.That(conversation.NotificationId, Is.EqualTo(result.NotificationId));
            Assert.That(result.ErrorCode, Is.EqualTo(0));
        }

        [Test]
        public async Task SendImageAsync_WhenHaveBotId_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var imageUrl = "https://example.com/image.jpg";

            // Act
            await service.SendImageAsync(bot.BotId, chatId, imageUrl);

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Is<SendPhotoRequest>(_ => _.ChatId.Identifier == chatId));
        }

        [Test]
        public async Task SendImageAsync_WhenNotHaveBotId_ThrowException()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var imageUrl = "https://example.com/image.jpg";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.SendImageAsync(0, chatId, imageUrl));
        }

        [Test]
        public async Task ResendImageOnlyAsync_WhenCallWithNoError_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var imageUrl = "https://example.com/image.jpg";
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.FailedImages = new List<string> { imageUrl };

            // Act
            var result = await service.ResendImageOnlyAsync(conversation);

            // Assert
            Assert.That(result.ErrorCode, Is.EqualTo(0));
        }

        [Test]
        public async Task ResendImageOnlyAsync_WhenCallWithError_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var imageUrl = "https://example.com/image.jpg";
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.FailedImages = new List<string> { imageUrl };
            mockTeleClient.When(x => x.MakeRequestAsync(request: Arg.Any<SendPhotoRequest>())).Throw(new Exception("chat not found"));

            // Act
            var result = await service.ResendImageOnlyAsync(conversation);

            // Assert
            Assert.That(result.ErrorCode, Is.EqualTo(1));
        }

        [Test]
        public async Task SendMessageWithImagesAsync_WhenCallWithNoAttachments_CallSendMessageOnly()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.Attachments = null;
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;

            _sendIndirectMessageSettings.IsOnlySendIndirect = false;

            // Act
            await service.SendMessageWithImagesAsync(conversation);

            // Assert
            await mockTeleClient.Received(1).MakeRequestAsync(request: Arg.Is<SendMessageRequest>(_ => _.ChatId.Identifier == chatId));
            await mockTeleClient.DidNotReceive().MakeRequestAsync(request: Arg.Is<SendPhotoRequest>(_ => _.ChatId.Identifier == chatId));
        }

        [Test]
        public async Task SendMessageWithImagesAsync_WhenCallWithAttachments_CallSendPhoto()
        {
            // Arrange
            var service = CreateService();
            var bot = _botConfigurations.First();
            var mockTeleClient = Substitute.For<ITelegramBotClient>();
            bot.BotClient = mockTeleClient;
            var chatId = 123456789;
            var message = "Hello, World!";
            var conversation = _fixture.Create<ConversationInfo>();
            conversation.Attachments = new List<string> { "https://example.com/image.jpg" };
            conversation.ConversationId = chatId.ToString();
            conversation.BotConversationId = bot.BotId.ToString();
            conversation.Message = message;

            _sendIndirectMessageSettings.IsOnlySendIndirect = false;

            // Act
            await service.SendMessageWithImagesAsync(conversation);

            // Assert
            await mockTeleClient.Received().MakeRequestAsync(request: Arg.Is<SendPhotoRequest>(_ => _.ChatId.Identifier == chatId));
        }

        private void SetUpGetToken(ChatbotToken token)
        {
            _sendIndirectMessageSettings.AuthenticateUrl = "https://example.com/auth";
            _handler.When(HttpMethod.Post, _sendIndirectMessageSettings.AuthenticateUrl)
                .Respond("application/json", JsonConvert.SerializeObject(token));
        }

        private TelegramBotService CreateService()
        {
            _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_httpClient);
            var sendIndirectMsgSettingOptions = Substitute.For<IOptionsSnapshot<SendIndirectMessageSettings>>();
            sendIndirectMsgSettingOptions.Value.Returns(_sendIndirectMessageSettings);
            var proxySettingsOptions = Substitute.For<IOptionsSnapshot<ProxySettings>>();
            proxySettingsOptions.Value.Returns(_proxySettings);
            return new TelegramBotService(
                _httpClientFactory,
                _appSettings,
                sendIndirectMsgSettingOptions,
                proxySettingsOptions,
                _botConfigurations,
                _commonLogService,
                _conversationService);
        }

    }
}