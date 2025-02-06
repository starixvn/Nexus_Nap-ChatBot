namespace Fanex.NCB.AutoJobs.Core.Tests
{
    using Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.TicketInfo;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Tests.Services.Helper;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using NSubstitute;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class RefNoTicketServiceTests
    {
        private IConfiguration configuration;
        private IHttpClientFactory httpClientFactory;

        [Fact]
        public async Task GetTicketByRefNoAsync_WhenCalledHaveResult_ReturnExpectedResult()
        {
            var service = GetService();
            FakeHttpClientResponse(HttpStatusCode.OK);

            var actual = await service.GetTicketByRefNoAsync(123);

            Assert.NotNull(actual);
            Assert.Equal("0", actual.ErrorCode);
        }

        [Fact]
        public async Task GetTicketByRefNoAsync_WhenCalledFailedResult_ThrowException()
        {
            var service = GetService();
            FakeHttpClientResponse(HttpStatusCode.Forbidden);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await service.GetTicketByRefNoAsync(123));

            Assert.Contains("123", exception.Message);
        }

        #region Helpers

        private void FakeHttpClientResponse(HttpStatusCode status = HttpStatusCode.OK)
        {
            var fakeTokenReult = new TicketInfo
            {
                ErrorCode = "0",
                Description = "Test",
                Odds = "ABCD"
            };

            httpClientFactory.CreateClient().Returns(new HttpClient(new MockHttpMessageHandler(
                new HttpResponseMessage(status)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(fakeTokenReult))
                })));
        }

        private RefNoTicketService GetService()
        {
            configuration = Substitute.For<IConfiguration>();
            configuration.GetSection("NapSettings:Url").Value.Returns("http://test.com");
            configuration.GetSection("NapSettings:APIKey").Value.Returns("kJCrpsMovfqAWrlY1E5sV1+ZpP5/vndJ9gE/QcSbYac=");
            httpClientFactory = Substitute.For<IHttpClientFactory>();
            return new RefNoTicketService(configuration, httpClientFactory);
        }

        #endregion Helpers
    }
}