using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.NCB.CustomAspNetCoreModule.HealthChecks
{
    public class UriAvailableHealthCheck : IHealthCheck
    {
        private readonly string _uri;

        public UriAvailableHealthCheck(string uri)
        {
            _uri = uri;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_uri).ConfigureAwait(false);

                return IsConnected(response)
                        ? HealthCheckResult.Healthy()
                        : HealthCheckResult.Unhealthy(description: "Error occurred, the status code is:" + response.StatusCode);
            }
        }

        private static bool IsConnected(HttpResponseMessage response)
        {
            var statusCodesIndicateConnected = new[] { HttpStatusCode.Unauthorized, HttpStatusCode.MethodNotAllowed };
            return response.IsSuccessStatusCode || Array.IndexOf(statusCodesIndicateConnected, response.StatusCode) != -1;
        }
    }
}