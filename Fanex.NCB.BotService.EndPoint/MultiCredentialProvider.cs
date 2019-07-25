namespace Fanex.NCB.BotService.EndPoint
{
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MultiCredentialProvider : ICredentialProvider
    {
        private readonly Dictionary<string, string> Credentials;

        public MultiCredentialProvider(BotConfiguration botConfig)
        {
            Credentials = botConfig.Services.ToDictionary(t => ((EndpointService)t).AppId, t => ((EndpointService)t).AppPassword);
        }

        public Task<bool> IsValidAppIdAsync(string appId)
        {
            return Task.FromResult(Credentials.ContainsKey(appId));
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            return Task.FromResult(Credentials.ContainsKey(appId) ? Credentials[appId] : null);
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(!Credentials.Any());
        }
    }
}
