namespace Fanex.NCB.BotService.EndPoint;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;

public class MultiCredentialProvider : ICredentialProvider
{
    private readonly Dictionary<string, string> Credentials;

#pragma warning disable CS0618 // Type or member is obsolete

    public MultiCredentialProvider(BotConfiguration botConfig)
#pragma warning restore CS0618 // Type or member is obsolete
    {
#pragma warning disable CS0618 // Type or member is obsolete
        Credentials =
            botConfig.Services.ToDictionary(t => ((EndpointService)t).AppId, t => ((EndpointService)t).AppPassword);
#pragma warning restore CS0618 // Type or member is obsolete
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