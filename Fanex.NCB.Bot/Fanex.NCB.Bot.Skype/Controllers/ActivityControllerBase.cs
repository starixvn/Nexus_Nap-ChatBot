using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Fanex.NCB.Bot.Skype.Controllers
{
    public class ActivityControllerBase : BaseController
    {
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

        protected virtual async Task OnContactRelationUpdateActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    await OnContactBotAddedAsync(member, turnContext, cancellationToken);
                }
            }

            if (turnContext.Activity.MembersRemoved != null)
            {
                foreach (var member in turnContext.Activity.MembersRemoved)
                {
                    await OnContactBotRemovedAsync(member, turnContext, cancellationToken);
                }
            }
        }

        private Task OnContactBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected virtual Task OnContactBotRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
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
                        await OnBotRemovedAsync(member, turnContext, cancellationToken);
                    }
                    else
                    {
                        await OnMembeRemovedAsync(member, turnContext, cancellationToken);
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
                        await OnMemberAddedAsync(member, turnContext, cancellationToken);
                    }
                    else
                    {
                        await OnBotAddedAsync(member, turnContext, cancellationToken);
                    }
                }
            }

            if (!isUpdatedMember)
            {
               await OnUpdatedTopic(turnContext, cancellationToken);
            }
        }

        protected virtual Task OnUpdatedTopic(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task OnBotRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnBotAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnMembeRemovedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OnOtherActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
