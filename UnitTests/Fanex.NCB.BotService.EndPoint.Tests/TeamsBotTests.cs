namespace Fanex.NCB.BotService.EndPoint.Tests
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Criteria;
    using Core.Services.Interfaces;
    using Bots;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using NSubstitute;
    using Xunit;

    public class TeamsBotTests
    {
        private readonly IConversationService conversationService;
        private readonly ITurnContext<IConversationUpdateActivity> turnContextConversation;
        private readonly ITurnContext<IMessageActivity> turnContextMessage;

        public TeamsBotTests()
        {
            conversationService = Substitute.For<IConversationService>();
            turnContextConversation = Substitute.For<ITurnContext<IConversationUpdateActivity>>();
            turnContextMessage = Substitute.For<ITurnContext<IMessageActivity>>();
        }

        [Fact]
        public async Task OnMessageActivityAsync_WithOtherCommand_Donothing()
        {
            IActivity activity = new Activity
            {
                Text = "@test This is text",
                Recipient = new ChannelAccount { Id = "abc", Name = "@test" },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" },
                TopicName = "Topic name"
            };

            turnContextMessage.Activity.Returns(activity);
            SetupTeamsBot("OnMessageActivityAsync", turnContextMessage);

            await turnContextMessage.DidNotReceive().SendActivityAsync(Arg.Any<IActivity>());
        }

        private void SetupTeamsBot(string methodName, object context)
        {
            var type = typeof(TeamsBot);
            var licbot = Activator.CreateInstance(type, conversationService);
            var method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.Name == methodName && x.IsVirtual);
            var param = new object[] { context, new CancellationToken() };
            method.Invoke(licbot, param);
        }
    }
}