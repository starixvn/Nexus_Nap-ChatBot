namespace Fanex.NCB.Services.Tests.TestableClasses
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Microsoft.Extensions.Options;

    public class PushSignalServiceTestable : PushSignalService
    {
        public PushSignalServiceTestable(
               IOptionsSnapshot<SignalRSettings> signalRSettingOptions,
               IHttpClientFactory httpClientFactory,
               ILogger logger) : base(signalRSettingOptions, httpClientFactory, logger)
        { }

        public async Task<JwtToken> Public_GetTokenAsync()
        {
            return await base.GetTokenAsync();
        }
    }
}