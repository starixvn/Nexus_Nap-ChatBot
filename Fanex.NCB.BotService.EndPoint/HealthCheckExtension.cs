namespace Fanex.NCB.BotService.EndPoint
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using RestSharp;

    public class HealthCheckExtension : IHealthCheck
    {
        private readonly ILogger logger;
        private readonly AppSettings appSettings;
        private readonly IConfiguration configuration;
        private readonly IRestClient webClient;

        public HealthCheckExtension(AppSettings appSettings, ILogger logger, IConfiguration configuration)
        {
            this.appSettings = appSettings;
            this.logger = logger;
            this.configuration = configuration;
            webClient = new RestClient
            {
                Encoding = Encoding.UTF8,
                Proxy = new WebProxy(configuration.GetSection("ProxySettings:ProxyAddress").Value, true)
            };
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            webClient.BaseUrl = new Uri(appSettings.HealthCheckUrl);
            var request = new RestRequest(appSettings.HealthCheckUrl, Method.GET);
            var response = await webClient.ExecuteGetTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                string errMsg = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: \n\rStackTrace: ",
                $"{appSettings.SkypeServiceUrl} is inaccessable",
                "Chatbot for Licensee - Skype Bot Service",
                string.Empty
                );
                await logger.ErrorAsync(errMsg);

                return HealthCheckResult.Unhealthy();
            }

            return HealthCheckResult.Healthy();
        }
    }
}