namespace Fanex.NCB.Services.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Tests.Helper;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class PushSignalServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IHttpClientFactory httpClientFactory;
        private ILogger logger;

        #region OnInsertNotificationAsync

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task OnInsertNotificationAsync_NotificationIdsAreInvalid_DoNothing(string notificationIds)
        {
            // Arrange
            var service = GetService();

            // Act
            await service.OnInsertNotificationAsync(notificationIds);

            // Assert
            service.DidNotReceive().Protected("GetTokenAsync");
        }

        [Fact]
        public async Task OnInsertNotificationAsync_RequestIsUnsuccessful_ExpectedWriteLog()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnInsertNotificationAsync("1");

            // Assert
            await logger.Received().ErrorAsync("Send OnInsertNotificationAsync fail - Status Code: BadRequest");
        }

        [Fact]
        public async Task OnInsertNotificationAsync_RequestIsSuccessful_DoNothing()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnInsertNotificationAsync("1");

            // Assert
            await logger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        #endregion OnInsertNotificationAsync

        #region OnUpdateNotificationStatusAsync

        [Theory]
        [InlineData(null)]
        public async Task OnUpdateNotificationStatusAsync_NotificationIdsAreInvalid_DoNothing(string notificationIds)
        {
            // Arrange
            var service = GetService();

            // Act
            await service.OnUpdateNotificationStatusAsync(notificationIds);

            // Assert
            service.DidNotReceive().Protected("GetTokenAsync");
        }

        [Fact]
        public async Task OnUpdateNotificationStatusAsync_RequestIsUnsuccessful_ExpectedWriteLog()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnUpdateNotificationStatusAsync("1");

            // Assert
            await logger.Received().ErrorAsync("Send OnUpdateNotificationStatusAsync fail - Status Code: BadRequest");
        }

        [Fact]
        public async Task OnUpdateNotificationStatusAsync_RequestIsSuccessful_DoNothing()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnUpdateNotificationStatusAsync("1");

            // Assert
            await logger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        #endregion OnUpdateNotificationStatusAsync

        #region OnHasNewErrNotificationAsync

        [Fact]
        public async Task OnHasNewErrNotificationAsync_RequestIsUnsuccessful_ExpectedWriteLog()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnHasNewErrNotificationAsync();

            // Assert
            await logger.Received().ErrorAsync("Send OnHasNewErrNotificationAsync fail - Status Code: BadRequest");
        }

        [Fact]
        public async Task OnHasNewErrNotificationAsync_RequestIsSuccessful_DoNothing()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);
            service.Protected("GetTokenAsync").Returns(Task.FromResult(token));

            // Act
            await service.OnHasNewErrNotificationAsync();

            // Assert
            await logger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        #endregion OnHasNewErrNotificationAsync

        [Fact]
        public async Task GetTokenAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var token = fixture.Create<JwtToken>();
            var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(token))
            });
            var httpClient = new HttpClient(httpMessageHandler);
            var service = GetService();
            httpClientFactory.CreateClient().Returns(httpClient);

            // Act
            var result = await service.Public_GetTokenAsync();

            // Assert
            Assert.Equal(token.TokenType, result.TokenType);
            Assert.Equal(token.ExpiresIn, result.ExpiresIn);
            Assert.Equal(token.AccessToken, result.AccessToken);
        }

        #region Helpers

        private PushSignalServiceTestable GetService()
        {
            var signalRSettings = new SignalRSettings
            {
                DomainURL = "https://abc.com",
                TokenURL = "/token",
                UpdateNotificationStatusURL = "/Update",
                InsertNotificationURL = "/Insert",
                ErrNotificationURL = "/Error",
                AppId = "sa",
                AppPassword = "1234aa"
            };
            var signalRSettingOptions = Substitute.For<IOptionsSnapshot<SignalRSettings>>();
            signalRSettingOptions.Value.Returns(signalRSettings);

            httpClientFactory = Substitute.For<IHttpClientFactory>();
            logger = Substitute.For<ILogger>();

            return Substitute.For<PushSignalServiceTestable>(signalRSettingOptions, httpClientFactory, logger);
        }

        #endregion Helpers
    }
}