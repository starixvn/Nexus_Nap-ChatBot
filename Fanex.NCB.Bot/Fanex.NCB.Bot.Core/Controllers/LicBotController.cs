using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Core.Services;
using Fanex.NCB.Bot.CoreControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.Core.Controllers
{
    [Route("LicBot")]
    public class LicBotController : ActivityControllerBase
    {
        private readonly IBotAdapterFactory _adapterFactory;
        public LicBotController(BotConfiguration botConfig,
            IBotConversationRepository botConversationRepository,
            IBotConversationMemberRepository botConversationMemberRepository,
            IBotAdapterFactory adapterFactory
            )
            : base(botConfig, botConversationRepository, botConversationMemberRepository)
        {
            _adapterFactory = adapterFactory;
        }

        protected override IAdapterIntegration CreateAdapter(string botName)
        {
            return _adapterFactory.Create(BotConfig, botName);
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
