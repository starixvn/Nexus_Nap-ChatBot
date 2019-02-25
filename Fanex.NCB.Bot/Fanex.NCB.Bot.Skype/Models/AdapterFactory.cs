using System;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;

namespace Fanex.NCB.Bot.Skype.Models
{
    public static class AdapterFactory
    {
        public static IAdapterIntegration Create(BotConfiguration botConfig, string name)
        {
            var endpointName = $"{name}";

            var service = botConfig.Services.Find(s => s.Type == "endpoint" && s.Name == endpointName);
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{name}'.");
            }

            var credentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

            var adapter = new BotFrameworkAdapter(credentialProvider)
            {
                OnTurnError = async (context, _) =>
                {
                    await context.SendActivityAsync($"Sorry, it looks like something went wrong in {name}.").ConfigureAwait(false);
                }
            };
            return adapter;
        }
    }
}
