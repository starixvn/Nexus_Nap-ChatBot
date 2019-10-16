namespace Fanex.NCB.BotService.EndPoint.Bots
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class LicBot : ActivityHandler
    {
        private const string channelSkypeId = "skype";
        private const string greetingMessage = "Hello, I am {0}";

        private readonly IConversationService conversationService;
        private readonly ILogger logger;

        public LicBot(IConversationService conversationService, ILogger logger)
        {
            this.conversationService = conversationService;
            this.logger = logger;
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await logger.WarnAsync("test 1");
            if (turnContext.Activity.ChannelId.Equals(channelSkypeId))
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    if (turnContext.Activity.MembersAdded.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                    {
                        await OnBotAddedAsync(turnContext, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    if (turnContext.Activity.MembersRemoved != null
                    && turnContext.Activity.MembersRemoved.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                    {
                        await OnBotRemovedAsync(turnContext, cancellationToken).ConfigureAwait(false);
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
            try
            {
                await logger.WarnAsync("test 12");

                var command = new AddBotToGroupCommand
                {
                    BotConversationId = turnContext.Activity?.Recipient.Id,
                    BotName = turnContext.Activity?.Recipient.Name,
                    ConversationId = turnContext.Activity?.Conversation.Id,
                    ChannelId = turnContext.Activity?.ChannelId,
                    ServiceUrl = turnContext.Activity?.ServiceUrl,
                    UpdatedBy = turnContext.Activity?.From.Id
                };

                await conversationService.AddBotToGroup(command);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message);
            }
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