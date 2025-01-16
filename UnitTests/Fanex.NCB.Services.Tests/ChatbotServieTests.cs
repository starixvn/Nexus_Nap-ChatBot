namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Models;
    using Models.NotifictaionSendMessage;
    using Helper;
    using TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class ChatbotServieTests
    {
        private readonly IFixture fixture = new Fixture();
        private IHttpClientFactory httpClientFactory;
        private AppSettings appSettings;
        private BotSettings botSettings;
        private MonitorChatbot monitorChatBot;
        private NotifyChatGroupOption notifyChatGroupOptions;
        private MonitorTelegramBot monitorTelegramBot;

        #region SendMultiMessageAsync

        [Fact]
        public async Task SendMultiMessageAsync_MessageIsNullOrEmpty_ReturnAsExpectedResult()
        {
            // Arrange
            byte channelId = 1;
            var messages = new List<NotificationMessage>();
            var service = GetService();

            // Act
            var result = await service.SendMultiMessageAsync(channelId, messages);

            // Assert
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal((HttpStatusCode)0, result.StatusCode);
            Assert.Empty(result.NotificationMessageResult);
        }

        [Fact]
        public async Task SendMultiMessageAsync_RequestIsSuccessful_ReturnAsExpectedResult()
        {
            // Arrange
            byte channelId = 1;
            var messages = fixture.Create<IEnumerable<NotificationMessage>>();
            var sendMessageResults = fixture.Create<List<SendMessageResult>>();
            var token = fixture.Create<ChatbotToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(sendMessageResults))
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            var result = await service.SendMultiMessageAsync(channelId, messages);

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(sendMessageResults.Count, result.NotificationMessageResult.Count());
        }

        #endregion SendMultiMessageAsync

        [Fact]
        public void GenerateNotifificationMessage_WhenCalled_ReturnMappedObject()
        {
            // Arrange
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage {  NotificationId = 10,  Content = fixture.Create<string>()}
            };
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 10,
                    BotConversationId = fixture.Create<string>(),
                    BotName = fixture.Create<string>(),
                    ServiceUrl = fixture.Create<string>(),
                    ChatGroupConversationId = fixture.Create<string>(),
                }
            };
            var expected = new List<NotificationMessage>
            {
                new NotificationMessage
                {
                    NotificationId = 10,
                    Message = notifications.FirstOrDefault().Content,
                    Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo
                        {
                            ConversationId = recipients.FirstOrDefault().ChatGroupConversationId,
                            BotConversationId = recipients.FirstOrDefault().BotConversationId,
                            BotName = recipients.FirstOrDefault().BotName,
                            ServiceUrl = recipients.FirstOrDefault().ServiceUrl
                        },
                    }
                }
            };
            var service = GetService();

            // Act
            var actual = service.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public async Task MonitorChatbotAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var service = GetService();

            // Act
            await service.MonitorChatbotAsync();

            // Assert
            service.Received().Protected("SendSingleMessageAsync", (byte)1,
                Arg.Is<ConversationInfo>(x => x.BotName == "1on1"
                    && x.BotConversationId == "ABC"
                    && x.ChannelId == "ACV"
                    && x.ConversationId == "Conversation"
                    && x.ServiceUrl == "test.com"
                    && x.Message == "message"));
        }

        [Fact]
        public async Task MonitorTelegramBotAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var token = fixture.Create<ChatbotToken>();
            var service = GetService();
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.MonitorTelegramBotAsync();

            // Assert
            service.Received().Protected("SendSingleMessageAsync", (byte)2,
                Arg.Is<ConversationInfo>(x => x.ConversationId == "1" && x.BotConversationId == "1" && x.Message == "message"));
        }

        [Fact]
        public async Task NotifyToChatGroupAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var token = fixture.Create<ChatbotToken>();
            var message = fixture.Create<string>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fixture.Create<IEnumerable<SendMessageResult>>()))
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.NotifyToChatGroupAsync(message);

            // Assert
            await service.Received().SendMultiMessageAsync((byte)1,
                Arg.Is<IEnumerable<NotificationMessage>>(x =>
                    x.Count() == 1
                    && x.FirstOrDefault().NotificationId == -1
                    && x.FirstOrDefault().Message == message
                    && x.FirstOrDefault().Recipients.Count() == 1
                    && x.FirstOrDefault().Recipients.FirstOrDefault().ConversationId == notifyChatGroupOptions.ConversationId
                    && x.FirstOrDefault().Recipients.FirstOrDefault().ChannelId == notifyChatGroupOptions.ChannelId
                    && x.FirstOrDefault().Recipients.FirstOrDefault().ServiceUrl == notifyChatGroupOptions.ServiceUrl
                    && x.FirstOrDefault().Recipients.FirstOrDefault().BotConversationId == notifyChatGroupOptions.BotConversationId
                    && x.FirstOrDefault().Recipients.FirstOrDefault().BotName == notifyChatGroupOptions.BotName));
        }

        [Fact]
        public async Task SendSingleMessageAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            byte channelId = 1;
            var conversationInfo = fixture.Create<ConversationInfo>();
            var httpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
            var token = fixture.Create<ChatbotToken>();
            var httpMessageHandler = new MockHttpMessageHandler(httpResponse);
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            var result = await service.Public_SendSingleMessageAsync(channelId, conversationInfo);

            // Assert
            Assert.Equal(httpResponse, result);
        }

        #region Helpers

        private ChatbotServiceTestable GetService()
        {
            notifyChatGroupOptions = new NotifyChatGroupOption
            {
                BotName = "1on1",
                BotConversationId = "ABC",
                ChannelId = "ACV",
                ConversationId = "Conversation",
                ServiceUrl = "test.com"
            };
            var sponGroupOptions = Substitute.For<IOptionsSnapshot<NotifyChatGroupOption>>();
            sponGroupOptions.Value.Returns(notifyChatGroupOptions);

            appSettings = new AppSettings { HttpClientTimeout = 200 };
            var appSettingOptions = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettingOptions.Value.Returns(appSettings);

            botSettings = new BotSettings
            {
                BotDomain = "https://test.com",
                AuthenticateUrl = "/AuthenticateUrl",
                ApiConfigs = new List<BotServiceEndPointConfigs>
                {
                    new  BotServiceEndPointConfigs {  ChannelId = 1, SendMessageUrl = "/abc", SendMultiMessageUrl = "/cde"},
                    new  BotServiceEndPointConfigs {  ChannelId = 2, SendMessageUrl = "/abc", SendMultiMessageUrl = "/cde"}
                }
            };
            var botSettingOptions = Substitute.For<IOptionsSnapshot<BotSettings>>();
            botSettingOptions.Value.Returns(botSettings);

            monitorChatBot = new MonitorChatbot
            {
                BotName = "1on1",
                BotConversationId = "ABC",
                ChannelId = "ACV",
                ConversationId = "Conversation",
                ServiceUrl = "test.com",
                Message = "message"
            };
            var monitorChatBotOptions = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
            monitorChatBotOptions.Value.Returns(monitorChatBot);

            monitorTelegramBot = new MonitorTelegramBot
            {
                BotId = 1,
                ChatId = 1,
                Message = "message"
            };
            var monitorTelegramBotOptions = Substitute.For<IOptionsSnapshot<MonitorTelegramBot>>();
            monitorTelegramBotOptions.Value.Returns(monitorTelegramBot);

            httpClientFactory = Substitute.For<IHttpClientFactory>();

            return Substitute.For<ChatbotServiceTestable>(
                httpClientFactory,
                appSettingOptions,
                botSettingOptions,
                monitorChatBotOptions,
                sponGroupOptions,
                monitorTelegramBotOptions);
        }

        #endregion Helpers
    }
}