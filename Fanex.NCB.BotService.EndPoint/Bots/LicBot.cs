namespace Fanex.NCB.BotService.EndPoint.Bots
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LicBot : ActivityHandler
    {
        private readonly IConversationService conversationService;

        public LicBot(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                if (turnContext.Activity.MembersAdded.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                {
                    await OnBotAddedAsync(turnContext, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (turnContext.Activity.MembersRemoved != null
                && turnContext.Activity.MembersRemoved.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
            {
                await OnBotRemovedAsync(turnContext, cancellationToken).ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(turnContext.Activity.TopicName))
            {
                await OnUpdatedTopic(turnContext);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected virtual async Task OnBotAddedAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var command = new AddBotToGroupCommand
            {
                BotConversationId = turnContext.Activity.Recipient.Id,
                BotName = turnContext.Activity.Recipient.Name,
                ConversatioId = turnContext.Activity.Conversation.Id,
                GroupName = turnContext.Activity.Conversation.Name,
                ChannelId = turnContext.Activity.ChannelId,
                ServiceUrl = turnContext.Activity.ServiceUrl,
                UpdatedBy = turnContext.Activity.From.Id
            };

            await conversationService.AddBotToGroup(command);
        }

        protected virtual async Task OnBotRemovedAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = turnContext.Activity.Recipient.Id,
                ConversatioId = turnContext.Activity.Conversation.Id,
                UpdatedBy = turnContext.Activity.From.Id
            };

            await conversationService.RemoveBotOutGroup(command);
        }

        private async Task OnUpdatedTopic(ITurnContext turnContext)
        {
            var command = new UpdateGroupTopicCommand
            {
                ConversatioId = turnContext.Activity.Conversation.Id,
                GroupName = turnContext.Activity.TopicName,
                UpdatedBy = turnContext.Activity.From.Id
            };

            await conversationService.UpdateGroupTopic(command);
        }
    }
}
