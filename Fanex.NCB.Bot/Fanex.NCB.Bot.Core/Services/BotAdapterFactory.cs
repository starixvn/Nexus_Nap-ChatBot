using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace Fanex.NCB.Bot.Core.Services
{
    public class BotAdapterFactory : IBotAdapterFactory
    {
        private readonly ILogger<BotAdapterFactory> _logger;
        public BotAdapterFactory(ILogger<BotAdapterFactory> logger)
        {
            _logger = logger;
        }
        public IAdapterIntegration Create(BotConfiguration botConfig, string name)
        {
            var endpointName = $"{name}";

            var service = botConfig.Services.Find(s => s.Type == "endpoint" && s.Name == endpointName);
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{name}'.");
            }

            var credentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

            return new BotFrameworkAdapter(credentialProvider)
            {
                OnTurnError = async (context, ex) =>
                {
                    _logger.LogCritical(ex.Message, ex);
                    await context.SendActivityAsync($"Sorry, it looks like something went wrong in {name}.").ConfigureAwait(false);
                }
            };
        }
    }
}
