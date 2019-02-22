using System;
using System.Collections.Generic;
using System.Text;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Skype.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;

namespace Fanex.NCB.Bot.Skype.Controllers
{
    [Route("OWnotify")]
    public class OWNotifyController:ActivityControllerBase
    {
        private readonly BotConfiguration _botConfig;
        public OWNotifyController(BotConfiguration botConfig, IBotConversationRepository botConversationRepository)
            : base(botConversationRepository)
        {
            _botConfig = botConfig;
        }

        protected override IAdapterIntegration CreateAdapter()
        {
            return AdapterFactory.Create(_botConfig, "OWnotify");
        }
    }
}
