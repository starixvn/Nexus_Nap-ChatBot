using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Fanex.NCB.CustomAspNetCoreModule.HealthChecks;

namespace Fanex.NCB.CustomAspNetCoreModule.Extensions
{
    public static class HealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddUriAvailable(this IHealthChecksBuilder builder, string uri, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(name,new UriAvailableHealthCheck(uri),failureStatus,tags));
        }

    }
}
