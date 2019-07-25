namespace Fanex.NCB.BotService.EndPoint.Bots
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class LicBot : ActivityHandler
    {
        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                if (turnContext.Activity.MembersAdded.Any(m => m.Id != turnContext.Activity.Recipient?.Id))
                {
                    await OnMembersAddedAsync(turnContext.Activity.MembersAdded, turnContext, cancellationToken);
                }
            }
            else if (turnContext.Activity.MembersRemoved != null)
            {
                if (turnContext.Activity.MembersRemoved.Any(m => m.Id != turnContext.Activity.Recipient?.Id))
                {
                    await OnMembersRemovedAsync(turnContext.Activity.MembersRemoved, turnContext, cancellationToken);
                }
            }

            if (!string.IsNullOrWhiteSpace(turnContext.Activity.TopicName))
            {
                await OnUpdatedTopic(turnContext, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }

        protected override async Task OnMembersRemovedAsync(IList<ChannelAccount> membersRemoved, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: 3"), cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: 1"), cancellationToken);
        }

        protected override async Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: 2"), cancellationToken);
        }

        protected override async Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: 4"), cancellationToken);
        }

        protected override async Task OnUnrecognizedActivityTypeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: 5"), cancellationToken);
        }


        private async Task OnUpdatedTopic(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //var item = await _botConversationRepository
            //   .SingleAsync(x => x.ConversationID == turnContext.Activity.Conversation.Id).ConfigureAwait(false);
            //if (item == null)
            //{
            //    item = NewBotConversation(turnContext);
            //    await _botConversationRepository.InsertAsync(item).ConfigureAwait(false);
            //}
            //else
            //{
            //    item.Name = turnContext.Activity.TopicName;
            //    item.LastUpdate = DateTime.Now;
            //    item.UpdatedBy = turnContext.Activity.Recipient.Name;
            //    item.ChannelID = turnContext.Activity.ChannelId;
            //    item.ServiceUrl = turnContext.Activity.ServiceUrl;
            //    await _botConversationRepository.UpdateAsync(item).ConfigureAwait(false);
            //}
        }



    }
}
