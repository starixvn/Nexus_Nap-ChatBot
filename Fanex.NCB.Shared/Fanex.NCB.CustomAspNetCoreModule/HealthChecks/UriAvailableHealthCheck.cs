using System;
using System.Collections.Generic;
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

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                var response = Task.Run<HttpResponseMessage>(async () => await client.GetAsync(_uri)).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Task.FromResult(HealthCheckResult.Healthy());
                }
                
                return Task.FromResult(HealthCheckResult.Unhealthy(description: "Error occurred, the status code is:" + response.StatusCode));
            }
        }
    }
}
