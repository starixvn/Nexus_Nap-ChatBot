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
    [Route("SpartaBot")]
    public class SpartaBotController : ActivityControllerBase
    {
        private readonly BotConfiguration _botConfig;
        private readonly IBotConversationRepository _botConversationRepository;
        public SpartaBotController(BotConfiguration botConfig, IBotConversationRepository botConversationRepository)
        {
            _botConfig = botConfig;
            _botConversationRepository = botConversationRepository;
        }

        protected override IAdapterIntegration CreateAdapter()
        {
            return AdapterFactory.Create(_botConfig, "SpartaBot");
        }


        protected override async Task OnBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var conversationInfo = await _botConversationRepository.SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id);
            if (conversationInfo == null)
            {
                conversationInfo = NewBotConversation(turnContext);
                if (string.IsNullOrEmpty(conversationInfo.Name))
                {
                    conversationInfo.Name = "null";
                }
                await _botConversationRepository.InsertAsync(conversationInfo);
            }

            await turnContext.SendActivityAsync(MessageFactory.Text("Hello, I'm Sparta Bot"));
        }

        private static BotConversation NewBotConversation(ITurnContext turnContext)
        {
            return new BotConversation
            {
                BotName = turnContext.Activity.Recipient.Name,
                IsBotActive = true,
                ChannelID = turnContext.Activity.ChannelId,
                ConversationID = turnContext.Activity.Conversation.Id,
                LastUpdate = DateTime.Now,
                Name = turnContext.Activity.TopicName

            };
        }

        protected override async Task OnUpdatedTopic(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var item = await _botConversationRepository
                .SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id);
            if (item == null)
            {
                item = NewBotConversation(turnContext);
                await _botConversationRepository.InsertAsync(item);
            }
            else
            {
                item.Name = turnContext.Activity.TopicName;
                item.IsBotActive = true;
                item.LastUpdate = DateTime.Now;
                item.BotName = turnContext.Activity.Recipient.Name;
                item.ChannelID = turnContext.Activity.ChannelId;
                await _botConversationRepository.UpdateAsync(item);
            }

        }
        protected override async Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo '{turnContext.Activity.Text}' Hello, I'm Sparta Bot"));
        }

        protected override async Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome {member.Name} to Sparta Team."), cancellationToken);
        }


    }
}
