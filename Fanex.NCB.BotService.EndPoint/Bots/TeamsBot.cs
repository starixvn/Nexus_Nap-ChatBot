using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Fanex.NCB.BotService.Core.Criteria;
using Fanex.NCB.BotService.Core.Enum;
using Fanex.NCB.BotService.Core.Services.Interfaces;
using System;
using Fanex.NCB.BotService.Core;
using System.Web;

namespace Fanex.NCB.BotService.EndPoint.Bots
{
    public class TeamsBot : TeamsActivityHandler
    {
        private readonly IConversationService _conversationService;

        public TeamsBot(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        protected override async Task OnTeamsMembersAddedAsync(IList<TeamsChannelAccount> teamsMembersAdded,
            TeamInfo teamInfo,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var teamMember in teamsMembersAdded)
            {
                if (teamMember.Id == turnContext.Activity.Recipient.Id
                    && IsGroupChatActivity(turnContext.Activity))
                {
                    await OnBotAddedAsync(turnContext);
                }
                if (teamMember.Id != turnContext.Activity.Recipient.Id && !IsPersonalActivity(turnContext.Activity))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to the team {teamMember.GivenName} {teamMember.Surname}."), cancellationToken);
                }
            }
        }

        protected override async Task OnMembersRemovedAsync(IList<ChannelAccount> membersRemoved,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var teamMember in membersRemoved)
            {
                if (teamMember.Id == turnContext.Activity.Recipient.Id && IsGroupChatActivity(turnContext.Activity))
                {
                    await OnBotDeleteAsync(turnContext);
                }
            }
        }

        protected override async Task OnInstallationUpdateAddAsync(ITurnContext<IInstallationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if(IsGroupChatActivity(turnContext.Activity))
            {
                await OnBotAddedAsync(turnContext);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity;
            if (activity.MentionsRecipient())
            {
                activity.RemoveRecipientMention();
                var text = activity.Text.Trim().ToLower(CultureInfo.InvariantCulture);
                var responseActivity = GetResponseActivity(text, turnContext);
                await turnContext.SendActivityAsync(responseActivity, cancellationToken);
            }

        }

        private IActivity GetResponseActivity(string text, ITurnContext turnContext) =>
            text switch
            {
                "hello" => MessageFactory.Text("Hello!"),
                "hi" => MessageFactory.Text("Hi!"),
                InteractedCommand.help => GetHelpSuggestedActivity(turnContext),
                InteractedCommand.getid => MessageFactory.Text(turnContext.Activity.Recipient.Id),
                InteractedCommand.groupid => MessageFactory.Text(turnContext.Activity.Conversation.Id),
                _ => MessageFactory.Text("I'm sorry, I don't understand that command.")
            };

        private IActivity GetHelpSuggestedActivity(ITurnContext turnContext)
        {
            var activity = MessageFactory.Text("Did you mean:");

            var mention = new Mention
            {
                Mentioned = turnContext.Activity.Recipient,
            };
            activity.Attachments.Add(new HeroCard
            {
                Title = "Did you mean:",
                Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, "Get Bot Id", text: InteractedCommand.getid, value: $"{mention.Text} {InteractedCommand.getid}"),
                        new CardAction(ActionTypes.ImBack, "Get Group Id", text: InteractedCommand.groupid, value: $"{mention.Text} {InteractedCommand.groupid}")
                    }
            }.ToAttachment());
            return activity;
        }

        private async Task OnBotAddedAsync(ITurnContext turnContext)
        {
            var command = new AddBotToGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = turnContext.Activity?.Conversation.Name,
                ChannelId = (byte)ConversationChannel.Teams,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await _conversationService.AddBotToGroup(command);
            await OnUpdatedTopic(turnContext);
        }

        private async Task OnBotDeleteAsync(ITurnContext turnContext)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id,
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await _conversationService.RemoveBotOutGroup(command);
        }

        private async Task OnUpdatedTopic(ITurnContext turnContext)
        {
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = HttpUtility.HtmlDecode(turnContext.Activity?.Conversation.Name),
                UpdatedBy = turnContext.Activity?.From.Id
            };

            await _conversationService.UpdateGroupTopic(command);
        }

        private bool IsGroupChatActivity(IActivity activity)
            => activity.Conversation.ConversationType.Equals("groupChat", StringComparison.OrdinalIgnoreCase);

        private bool IsPersonalActivity(IActivity activity)
            => activity.Conversation.ConversationType.Equals("personal", StringComparison.OrdinalIgnoreCase);
    }
}
