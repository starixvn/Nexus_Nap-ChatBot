namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class ChatbotServieTests
    {
        private IRestClient webClient;
        private IOptionsSnapshot<BotSettings> botSettings;

        [Fact]
        public async Task SendMessageToMultitpleAsync_WhenCalled_ExecutePostTaskAsync()
        {
            var messages = new List<NotificationMessage>();
            var expected = new RestResponse
            {
                StatusCode = HttpStatusCode.OK
            };
            var service = GetService();
            var request = new RestRequest("/Test", Method.POST);

            webClient.ExecutePostTaskAsync(Arg.Is(request)).Returns(expected);

            //Act
            var actual = await service.SendMessageToMultitpleAsync(messages);

            //Assert
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
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

        private ChatbotService GetService()
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            var settings = new BotSettings
            {
                BotDomain = "http://test.com",
                BotServiceUrl = "/Test",
                TokenUrl = "ABC/test"
            };
            webClient = Substitute.For<IRestClient>();
            botSettings = Substitute.For<IOptionsSnapshot<BotSettings>>();
            botSettings.Value.Returns(settings);

            return new ChatbotService(botSettings);
        }
    }
}