using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;
using Fanex.NCB.Bot.Skype.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.Skype.Controllers
{
    [Route("LicBot")]
    public class LicBotController : ActivityControllerBase
    {
        private readonly BotConfiguration _botConfig;
        public LicBotController(BotConfiguration botConfig, IBotConversationRepository botConversationRepository)
            : base(botConversationRepository)
        {
            _botConfig = botConfig;
        }

        protected override IAdapterIntegration CreateAdapter(string botName)
        {
            return AdapterFactory.Create(_botConfig, botName);
        }


        protected override async Task OnBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await base.OnBotAddedAsync(member, turnContext, cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text("Hello, I'm Sparta Bot")).ConfigureAwait(false);
        }


        protected override async Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo '{turnContext.Activity.Text}' Hello, I'm Sparta Bot")).ConfigureAwait(false);
        }

        protected override async Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome {member.Name} to Sparta Team."), cancellationToken).ConfigureAwait(false);
        }


    }
}
