using System;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NCB.Bot.BaseStorage;
using Fanex.NCB.Bot.Models.DTO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.Skype.Controllers
{
    public class ActivityControllerBase : BaseController
    {
        private readonly IBotConversationRepository _botConversationRepository;

        public ActivityControllerBase(IBotConversationRepository botConversationRepository)
        {
            _botConversationRepository = botConversationRepository;
        }
        protected override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:

                    return OnMessageActivityAsync(turnContext, cancellationToken);
                case ActivityTypes.ConversationUpdate:
                    return OnConversationUpdateActivityAsync(turnContext, cancellationToken);
                case ActivityTypes.ContactRelationUpdate:
                    return OnContactRelationUpdateActivityAsync(turnContext, cancellationToken);
                default:
                    return OnOtherActivityAsync(turnContext, cancellationToken);
            }

        }
        #region  OnContactRelationUpdate
        protected virtual async Task OnContactRelationUpdateActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    await OnContactBotAddedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                }
            }

            if (turnContext.Activity.MembersRemoved != null)
            {
                foreach (var member in turnContext.Activity.MembersRemoved)
                {
                    await OnContactBotRemovedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        protected virtual Task OnContactBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual Task OnContactBotRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion
        protected virtual Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region  OnConversationUpdate
        private async Task OnConversationUpdateActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var isUpdatedMember = false;
            if (turnContext.Activity.MembersRemoved != null)
            {
                isUpdatedMember = true;
                foreach (var member in turnContext.Activity.MembersRemoved)
                {
                    if (member.Id == turnContext.Activity.Recipient.Id)
                    {
                        await OnBotRemovedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await OnMembeRemovedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            if (turnContext.Activity.MembersAdded != null)
            {
                isUpdatedMember = true;

                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {
                        await OnMemberAddedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await OnBotAddedAsync(member, turnContext, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            if (!isUpdatedMember)
            {
                await OnUpdatedTopic(turnContext, cancellationToken).ConfigureAwait(false);
            }
        }

        protected virtual async Task OnUpdatedTopic(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var item = await _botConversationRepository
               .SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id).ConfigureAwait(false);
            if (item == null)
            {
                item = NewBotConversation(turnContext);
                await _botConversationRepository.InsertAsync(item).ConfigureAwait(false);
            }
            else
            {
                item.Name = turnContext.Activity.TopicName;
                item.IsBotActive = true;
                item.LastUpdate = DateTime.Now;
                item.BotName = turnContext.Activity.Recipient.Name;
                item.ChannelID = turnContext.Activity.ChannelId;
                item.ServiceUrl = turnContext.Activity.ServiceUrl;
                await _botConversationRepository.UpdateAsync(item).ConfigureAwait(false);
            }
        }

        private async Task OnBotRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var conversationInfo = await _botConversationRepository.SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id).ConfigureAwait(false);
            if (conversationInfo == null)
            {
                conversationInfo = NewBotConversation(turnContext);
                if (string.IsNullOrEmpty(conversationInfo.Name))
                {
                    conversationInfo.Name = "null";
                }
                await _botConversationRepository.InsertAsync(conversationInfo).ConfigureAwait(false);
            }
        }

        protected virtual async Task OnBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var conversationInfo = await _botConversationRepository.SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id);
            if (conversationInfo == null)
            {
                conversationInfo = NewBotConversation(turnContext);
                if (string.IsNullOrEmpty(conversationInfo.Name))
                {
                    conversationInfo.Name = "null";
                }
                await _botConversationRepository.InsertAsync(conversationInfo).ConfigureAwait(false);
            }
        }
        protected virtual Task OnMembeRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion
        protected virtual Task OnOtherActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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
                Name = turnContext.Activity.TopicName,
                ServiceUrl = turnContext.Activity.ServiceUrl
            };
        }
    }
}
