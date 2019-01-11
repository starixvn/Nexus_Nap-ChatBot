using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.Skype.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.Skype.Controllers
{
    [Route("bot1")]
    public class Bot1Controller : ActivityControllerBase
    {
        private readonly BotConfiguration _botConfig;

        public Bot1Controller(BotConfiguration botConfig)
        {
            _botConfig = botConfig;
        }

        protected override IAdapterIntegration CreateAdapter()
        {
            return AdapterFactory.Create(_botConfig, "bot1");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo '{turnContext.Activity.Text}' Licensee Bot1."));
        }

        protected override async Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome {member.Name} to Licensee Bot1."), cancellationToken);
        }

    }
}
