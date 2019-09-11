namespace Fanex.NCB.Services.Tests
{
    using Fanex.NCB.Core.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using RestSharp;

    public class ChatbotServieTests
    {
        private IRestClient webClient;
        private IOptionsSnapshot<BotSettings> botSettings;

        //[Fact]
        //public async Task SendMessageToMultitpleAsync_WhenCalled_ExecutePostTaskAsync()
        //{
        //    var messages = new List<NotificationMessage>();
        //    var expected = new RestResponse { StatusCode = HttpStatusCode.OK };
        //    var service = GetService();
        //    webClient = new RestClient();
        //    webClient.ExecuteAsyncPost(Arg.Any<RestRequest>).Returns(expected);
        //    Act
        //    var actual = await service.SendMessageToMultitpleAsync(messages);

        //    Assert

        //    Assert.Equal(new Uri("http://test.com"), webClient.BaseUrl);
        //}

        private ChatbotService GetService()
        {
            var settings = new BotSettings
            {
                BotDomain = "http://test.com",
                BotServiceUrl = "/Test"
            };
            botSettings = Substitute.For<IOptionsSnapshot<BotSettings>>();
            botSettings.Value.Returns(settings);
            return new ChatbotService(botSettings);
        }
    }
}