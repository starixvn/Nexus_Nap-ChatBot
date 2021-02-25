namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class ChatbotServieTests
    {
        private IRestClient webClient;
        private ILogService logService;
        private IOptionsSnapshot<BotSettings> botSettings;
        private IOptionsSnapshot<MonitorChatbot> monitor;
        private IOptionsSnapshot<NotifyChatGroupOption> notifyChatGroupOptions;
        private IOptionsSnapshot<List<HangfireJobSetting>> jobs;

        [Fact]
        public async Task SendMessageToMultitpleAsync_WhenCalled_ExecutePostTaskAsync()
        {
            var fixture = new Fixture();
            var messages = new List<NotificationMessage>();
            var expected = Substitute.For<IRestResponse>();
            expected.StatusCode.Returns(HttpStatusCode.OK);

            var tokenRes = Substitute.For<IRestResponse<ChatbotToken>>();
            tokenRes.StatusCode.Returns(HttpStatusCode.OK);
            tokenRes.Data.Returns(fixture.Create<ChatbotToken>());
            var service = GetService();
            var token = fixture.Create<ChatbotToken>();
            var resToken = Substitute.For<IRestResponse<ChatbotToken>>();
            resToken.Data = token;

            webClient.ExecutePostTaskAsync<ChatbotToken>(Arg.Any<RestRequest>()).Returns(tokenRes);
            webClient.ExecutePostTaskAsync(Arg.Any<RestRequest>()).Returns(expected);

            //Act
            var actual = await service.SendMessageToMultitpleAsync(messages);

            //Assert
            Assert.Equal("/Test", actual.Request.Resource);
        }

        [Fact]
        public async Task SendMessageAsync_EmptyConversationId_WebClientNotCalled()
        {
            var service = GetService();

            //Actual
            await service.SendMessageAsync(string.Empty, string.Empty, "Test");

            //Assert
            await webClient.DidNotReceiveWithAnyArgs()
                .ExecutePostTaskAsync(Arg.Any<RestRequest>());
        }

        [Fact]
        public async Task SendMessageAsync_ValidConversationId_WebClientExecuted()
        {
            var service = GetService();
            var expected = new RestResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var request = new RestRequest("/Test", Method.POST);

            webClient.ExecutePostTaskAsync(Arg.Do<RestRequest>(x => request = x)).Returns(expected);
            //Actual
            await service.SendMessageAsync("", "Test conversationId", "Test");

            //Assert
            webClient.ReceivedCalls();
        }

        [Fact]
        public async Task NotifySponDemoChatGroup_WhenCalled_WebClientExecuted()
        {
            var service = GetService();
            await service.NotifyToChatGroup("Test");
            webClient.Received(2);
        }

        [Fact]
        public async Task MonitorChatbot_WhenCalled_DoExpectedActions()
        {
            var service = GetService();
            await service.MonitorChatbot();

            webClient.ReceivedCalls();
        }

        [Fact]
        public void GenerateNotifificationMessage_WhenCalled_ReturnMappedObject()
        {
            var fixture = new Fixture();
            IEnumerable<NotificationSendMessage> notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage {  NotificationId = 10,  Content = fixture.Create<string>()}
            };

            IEnumerable<NotificationRecipient> recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 10,
                    BotConversationId = fixture.Create<string>(),
                    BotName = fixture.Create<string>(),
                    ServiceUrl = fixture.Create<string>(),
                    ChannelId = fixture.Create<string>(),
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
                            ChannelId = recipients.FirstOrDefault().ChannelId,
                            ServiceUrl = recipients.FirstOrDefault().ServiceUrl
                        },
                    }
                }
            };
            var service = GetService();
            var actual = service.GenerateNotifificationMessage(notifications, recipients);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        private ChatbotService GetService()
        {
            var fixture = new Fixture();
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            var settings = new BotSettings
            {
                BotDomain = "http://test.com",
                BotServiceUrl = "/Test",
                TokenUrl = "ABC/test"
            };
            var sponGroupOptions = new NotifyChatGroupOption
            {
                BotName = "1on1",
                BotConversationId = "ABC",
                ChannelId = "ACV",
                ConversationId = "Conversation",
                ServiceUrl = "test.com"
            };
            var monitorOptions = new MonitorChatbot
            {
                BotName = "1on1",
                BotConversationId = "ABC",
                ChannelId = "ACV",
                ConversationId = "Conversation",
                ServiceUrl = "test.com"
            };
            webClient = Substitute.For<IRestClient>();
            botSettings = Substitute.For<IOptionsSnapshot<BotSettings>>();
            notifyChatGroupOptions = Substitute.For<IOptionsSnapshot<NotifyChatGroupOption>>();
            logService = Substitute.For<ILogService>();
            monitor = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
            monitor.Value.Returns(monitorOptions);
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();

            var jobList = fixture.Create<List<HangfireJobSetting>>();
            jobList.Add(new HangfireJobSetting { JobName = HangfireJobName.MonitorSkypebot, IsEnabled = true });
            jobs.Value.Returns(jobList);
            botSettings.Value.Returns(settings);
            notifyChatGroupOptions.Value.Returns(sponGroupOptions);

            return new ChatbotService(logService, botSettings, monitor, notifyChatGroupOptions, jobs);
        }
    }
}