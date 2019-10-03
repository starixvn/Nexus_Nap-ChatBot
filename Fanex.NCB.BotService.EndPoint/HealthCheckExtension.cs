namespace Fanex.NCB.BotService.EndPoint
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using RestSharp;

    public class HealthCheckExtension : IHealthCheck
    {
        private readonly IRestClient webClient;
        private readonly AppSettings appSettings;

        public HealthCheckExtension(AppSettings appSettings)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.appSettings = appSettings;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}