namespace Fanex.NCB.BotService.EndPoint.Tests.Extensions
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.EndPoint.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class HealthCheckSkypeUriTests
    {
        private IRestClient webClient;
        private ILogger logger;
        private IConfiguration configuration;
        private AppSettings appSettings;

        [Fact]
        public async Task HealthCheck_WhenUnHealthy_ReturnExpectedStatusAndLoggerBeCalled()
        {
            var healthCheck = Setup();
            var response = Substitute.For<IRestResponse>();
            response.IsSuccessful.Returns(true);
            response.StatusCode = HttpStatusCode.InternalServerError;

            webClient.ExecuteGetTaskAsync(Arg.Any<RestRequest>()).Returns(response);
            //Actual
            var actual = await healthCheck.CheckHealthAsync(Arg.Any<HealthCheckContext>(), Arg.Any<CancellationToken>());

            //Asser
            Assert.IsType<HealthCheckResult>(actual);
            Assert.Equal(HealthStatus.Unhealthy, actual.Status);
        }

        private HealthCheckSkypeUri Setup()
        {
            configuration = Substitute.For<IConfiguration>();
            var section = Substitute.For<IConfigurationSection>();
            section.Value = "10.18.200.41:3128";
            configuration.GetSection(Arg.Any<string>()).Returns(section);
            logger = Substitute.For<ILogger>();
            webClient = Substitute.For<IRestClient>();
            appSettings = new AppSettings
            {
                HealthCheckUrl = "http://test.com"
            };

            return new HealthCheckSkypeUri(logger, configuration, appSettings);
        }
    }
}