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
        public LicBotController(BotConfiguration botConfig, IBotConversationRepository botConversationRepository, IBotConversationMemberRepository botConversationMemberRepository)
            : base(botConfig, botConversationRepository, botConversationMemberRepository)
        {

        }

        protected override IAdapterIntegration CreateAdapter(string botName)
        {
            return AdapterFactory.Create(BotConfig, botName);
        }

        protected override async Task OnBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var botTask = base.OnBotAddedAsync(member, turnContext, cancellationToken);
            var bot = BotConfig.Services.Find(x => x.Name == turnContext.Activity.Recipient.Name);
            var msgTask = bot != null ? turnContext.SendActivityAsync(MessageFactory.Text($"{bot.Properties.GetValue("Intro")}")) : Task.CompletedTask;

            await Task.WhenAll(botTask, msgTask).ConfigureAwait(false);
        }

    }
}
