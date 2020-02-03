namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class ChatbotServieTests
    {
        private IRestClient webClient;
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
            var fixture = new Fixture();
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
            monitor = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
            monitor.Value.Returns(monitorOptions);
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();
            jobs.Value.Returns(fixture.Create<List<HangfireJobSetting>>());
            botSettings.Value.Returns(settings);
            notifyChatGroupOptions.Value.Returns(sponGroupOptions);

            return new ChatbotService(botSettings, monitor, notifyChatGroupOptions, jobs);
        }
    }
}