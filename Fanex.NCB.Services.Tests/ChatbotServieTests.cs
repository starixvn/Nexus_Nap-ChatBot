namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class ChatbotServieTests
    {
        private readonly IRestClient webClient;
        private readonly IOptionsSnapshot<BotSettings> botSettings;

        public ChatbotServieTests()
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            var settings = new BotSettings
            {
                BotDomain = "http://test.com",
                BotServiceUrl = "/Test"
            };
            webClient = Substitute.For<IRestClient>();
            botSettings = Substitute.For<IOptionsSnapshot<BotSettings>>();
            botSettings.Value.Returns(settings);
        }

        [Fact]
        public async Task SendMessageToMultitpleAsync_WhenCalled_ExecutePostTaskAsync()
        {
            var messages = new List<NotificationMessage>();
            var expected = new RestResponse { StatusCode = HttpStatusCode.OK };
            var service = new ChatbotService(botSettings);
            var request = new RestRequest("/Test", Method.POST);
            request.AddHeader("Authorization", "Bearer ABCDEF");
            var chatbotToken = new ChatbotToken
            {
                AccessToken = "ABCDEF"
            };
            var token = new RestResponse<ChatbotToken>
            {
                Data = chatbotToken
            };
            webClient.ExecutePostTaskAsync<ChatbotToken>(Arg.Any<RestRequest>()).Returns(token);
            webClient.ExecutePostTaskAsync(Arg.Any<RestRequest>()).Returns(expected);

            //Act
            await service.SendMessageToMultitpleAsync(messages);

            //Assert
            await webClient.Received(2).ExecutePostTaskAsync(Arg.Any<RestRequest>());
        }
    }
}