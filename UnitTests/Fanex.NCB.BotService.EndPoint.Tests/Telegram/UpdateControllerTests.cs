namespace Fanex.NCB.BotService.EndPoint.Tests.Telegram
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram;
    using Fanex.NCB.Shared.Models;
    using global::Telegram.Bot.Types;
    using NSubstitute;
    using Xunit;

    public class UpdateControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private ILogger logger;
        private ITelegramBotService botService;
        private IConversationService conversationService;

        [Fact]
        public async Task EditChatTitledAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                NewChatTitle = "new chat title",
                From = new User { Id = 1 }
            };
            var command = new UpdateGroupTopicCommand();
            var controller = GetController();
            conversationService.UpdateGroupTopic(Arg.Do<UpdateGroupTopicCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_EditChatTitledAsync(message);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.NewChatTitle, command.GroupName);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        [Fact]
        public async Task AddBotToGroupAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1, Title = "chat title" },
                From = new User { Id = 1 }
            };
            var bot = fixture.Create<User>();
            var command = new AddBotToGroupCommand();
            var controller = GetController();
            conversationService.AddBotToGroup(Arg.Do<AddBotToGroupCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_AddBotToGroupAsync(message, bot);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(bot.Id.ToString(), command.BotConversationId);
            Assert.Equal(bot.FirstName, command.BotName);
            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.Chat.Title, command.GroupName);
            Assert.Equal(2, command.ChannelId);
            Assert.Empty(command.ServiceUrl);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        [Fact]
        public async Task RemoveBotToGroupAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                LeftChatMember = new User { Id = 2 },
                From = new User { Id = 1 }
            };
            var command = new RemoveBotOutGroupCommand();
            var controller = GetController();
            conversationService.RemoveBotOutGroup(Arg.Do<RemoveBotOutGroupCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_RemoveBotToGroupAsync(message);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.LeftChatMember.Id.ToString(), command.BotConversationId);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        #region Helpers

        private UpdateControllerTestable GetController()
        {
            logger = Substitute.For<ILogger>();
            botService = Substitute.For<ITelegramBotService>();
            conversationService = Substitute.For<IConversationService>();
            return new UpdateControllerTestable(logger, botService, conversationService);
        }

        #endregion Helpers
    }
}