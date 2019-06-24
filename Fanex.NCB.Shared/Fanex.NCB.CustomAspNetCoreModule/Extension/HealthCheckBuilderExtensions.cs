using Fanex.NCB.CustomAspNetCoreModule.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;

namespace Fanex.NCB.CustomAspNetCoreModule.Extensions
{
    public static class HealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddUriAvailable
            (this IHealthChecksBuilder builder, 
            string uri,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(name, new UriAvailableHealthCheck(uri), failureStatus, tags));
        }
    }
}