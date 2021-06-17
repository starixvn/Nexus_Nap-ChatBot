namespace Fanex.NCB.Services.Tests
{
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using System.Net;
    using System.Threading.Tasks;
    using Xunit;

    public class PushSignalServiceTests
    {
        //private readonly IRestClient restClient;
        //private readonly ILogger logger;
        //private readonly PushSignalService service;

        //public PushSignalServiceTests()
        //{
        //    var settings = new SignalRSettings
        //    {
        //        DomainURL = "https://abc.com",
        //        TokenURL = "cde.com",
        //        UpdateNotificationStatusURL = "/Update",
        //        InsertNotificationURL = "/Insert",
        //        ErrNotificationURL = "/Error",
        //        AppId = "sa",
        //        AppPassword = "1234aa"
        //    };
        //    var token = new JwtToken
        //    {
        //        TokenType = "Beare",
        //        AccessToken = "ABC"
        //    };
        //    var tokenResponse = Substitute.For<IRestResponse<JwtToken>>();
        //    tokenResponse.StatusCode = HttpStatusCode.OK;
        //    tokenResponse.Data.Returns(token);
        //    restClient = Substitute.For<IRestClient>();

        //    restClient.ExecutePostTaskAsync<JwtToken>(Arg.Any<RestRequest>()).Returns(Task.FromResult<IRestResponse<JwtToken>>(new RestResponse<JwtToken> { StatusCode = HttpStatusCode.OK, Data = token }));

        //    IOptionsSnapshot<SignalRSettings> signalRSettings = Substitute.For<IOptionsSnapshot<SignalRSettings>>();
        //    signalRSettings.Value.Returns(settings);
        //    logger = Substitute.For<ILogger>();
        //    service = new PushSignalService(signalRSettings, logger);
        //}

        //[Fact]
        //public async Task OnInsertNotification_WhenHaveNotificationIds_CallsExpectedRestClient()
        //{
        //    var notificationIds = "1,2,3,4";
        //    var response = Substitute.For<IRestResponse>();
        //    response.StatusCode = HttpStatusCode.OK;
        //    restClient.ExecutePostTaskAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = HttpStatusCode.OK }));
        //    await service.OnInsertNotification(notificationIds);
        //    await logger.Received().ErrorAsync(Arg.Any<string>());
        //}

        //[Fact]
        //public async Task OnUpdateNotificationStatus_WhenHaveNotificationIds_CallsExpectedRestClient()
        //{
        //    var notificationIds = "1,2,3,4";
        //    var response = Substitute.For<IRestResponse>();
        //    response.StatusCode = HttpStatusCode.OK;
        //    restClient.ExecutePostTaskAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult(response).Result);
        //    await service.OnUpdateNotificationStatus(notificationIds);
        //    await logger.Received().ErrorAsync(Arg.Any<string>());
        //}

        //[Fact]
        //public async Task OnHasNewErrNotifications_WhenHaveNotificationIds_CallsExpectedRestClient()
        //{
        //    var response = Substitute.For<IRestResponse>();
        //    response.StatusCode = HttpStatusCode.OK;
        //    restClient.ExecutePostTaskAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult(response).Result);
        //    await service.OnHasNewErrNotifications();
        //    await logger.Received().ErrorAsync(Arg.Any<string>());
        //}
    }
}