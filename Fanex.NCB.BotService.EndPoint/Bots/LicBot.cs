namespace Fanex.NCB.BotService.EndPoint.Bots
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Core;
    using Core.Criteria;
    using Core.Enum;
    using Core.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class LicBot : ActivityHandler
    {
        private const string channelSkypeId = "skype";
        private const string greetingMessage = "Hello, I am {0}";

        private readonly IConversationService conversationService;

        public LicBot(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.ChannelId.Equals(channelSkypeId))
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    if (turnContext.Activity.MembersAdded.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                    {
                        await OnBotAddedAsync(turnContext, cancellationToken);
                    }
                }
                else
                {
                    if (turnContext.Activity.MembersRemoved != null
                    && turnContext.Activity.MembersRemoved.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                    {
                        await OnBotRemovedAsync(turnContext, cancellationToken);
                    }
                }

                if (!string.IsNullOrWhiteSpace(turnContext.Activity.TopicName))
                {
                    await OnUpdatedTopic(turnContext);
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, turnContext.Activity.Recipient.Name)), cancellationToken);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = turnContext.Activity.Text.Trim();
            var botName = turnContext.Activity.Recipient.Name;
            if (turnContext.Activity.Conversation.IsGroup == true)
            {
                message = BotHelper.GetMessageInGroupByChannel(message, botName);
            }

            if (message.Equals(InteractedCommand.help, StringComparison.InvariantCultureIgnoreCase))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, botName)), cancellationToken);
            }
            else
            {
                if (message.Equals(InteractedCommand.getid, StringComparison.InvariantCultureIgnoreCase))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(turnContext.Activity.Conversation.Id), cancellationToken);
                }
            }
        }

        protected virtual async Task OnBotAddedAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var command = new AddBotToGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = turnContext.Activity?.TopicName,
                ChannelId = (byte)ConversationChannel.Skype,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await conversationService.AddBotToGroup(command);
        }

        protected virtual async Task OnBotRemovedAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id,
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await conversationService.RemoveBotOutGroup(command);
        }

        private async Task OnUpdatedTopic(ITurnContext turnContext)
        {
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = HttpUtility.HtmlDecode(turnContext.Activity?.TopicName),
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await conversationService.UpdateGroupTopic(command);
        }
    }
}