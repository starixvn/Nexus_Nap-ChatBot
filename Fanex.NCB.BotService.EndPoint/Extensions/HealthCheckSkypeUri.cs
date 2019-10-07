namespace Fanex.NCB.BotService.EndPoint.Extensions
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

    public class HealthCheckSkypeUri : IHealthCheck
    {
        private readonly IRestClient webClient;
        private readonly ILogger logger;
        private readonly AppSettings appSettings;

        public HealthCheckSkypeUri(ILogger logger, IConfiguration configuration, AppSettings appSettings)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            webClient = new RestClient
            {
                Encoding = Encoding.UTF8,
                Proxy = new WebProxy(configuration.GetSection("ProxySettings:ProxyAddress").Value, false)
            };
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            string uri = appSettings.HealthCheckUrl;
            webClient.BaseUrl = new Uri(uri);
            var request = new RestRequest(uri, Method.GET);
            try
            {
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
            }
            catch (Exception exception)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    string.Empty,
                    exception.TargetSite,
                    exception.StackTrace);
                await logger.ErrorAsync(message);
                throw;
            }

            return HealthCheckResult.Healthy();
        }
    }
}