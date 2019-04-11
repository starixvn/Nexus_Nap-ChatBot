using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fanex.NCB.CustomAspNetCoreModule.HealthChecks
{
    public class UriAvailableHealthCheck : IHealthCheck
    {
        private string _uri;

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
            var statusCodesIndicateConnected = new [] { HttpStatusCode.Unauthorized, HttpStatusCode.MethodNotAllowed };
            return response.IsSuccessStatusCode || Array.IndexOf(statusCodesIndicateConnected, response.StatusCode) != -1;
        }
    }
}
