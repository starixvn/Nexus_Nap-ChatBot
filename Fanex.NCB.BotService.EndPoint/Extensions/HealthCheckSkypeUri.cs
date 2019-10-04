namespace Fanex.NCB.BotService.EndPoint.Extensions
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using RestSharp;

    public class HealthCheckSkypeUri : IHealthCheck
    {
        private readonly IRestClient webClient;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public HealthCheckSkypeUri(ILogger logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            webClient = new RestClient
            {
                Encoding = Encoding.UTF8,
                Proxy = new WebProxy(configuration.GetSection("ProxySettings:ProxyAddress").Value, true)
            };
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            string uri = configuration.GetSection("AppSettings:HealthCheckUrl").Value;
            webClient.BaseUrl = new Uri(uri);
            var request = new RestRequest(uri, Method.GET);
            var response = await webClient.ExecuteGetTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string errMsg = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: \n\rStackTrace: ",
                $"{uri} is inaccessable",
                "Chatbot for Licensee - Skype Bot Service",
                string.Empty
                );
                await logger.ErrorAsync(errMsg);

                return HealthCheckResult.Unhealthy(description: "Microsoft Skype service Error occurred");
            }

            return HealthCheckResult.Healthy();
        }
    }
}