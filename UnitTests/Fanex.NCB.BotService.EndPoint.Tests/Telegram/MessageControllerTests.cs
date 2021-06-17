namespace Fanex.NCB.BotService.EndPoint.Tests.Telegram
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class MessageControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private AppSettings appSettings;
        private ILogger logger;
        private IConversationService conversationService;
        private ITelegramBotService botService;
        private IMessageLogService messageLogService;

        #region AddMessageLogToDatabaseAsync

        [Fact]
        public async Task AddMessageLogToDatabaseAsync_WhenCalled_CallCorrectlyMethod()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var messageContent = fixture.Create<string>();
            var result = fixture.Create<SendMessageResult>();
            var command = new AddMessageLogCommand();
            var commandResult = fixture.Create<CommandResult>();
            var controller = GetController();
            conversationService.AddMessageLog(Arg.Do<AddMessageLogCommand>(x => command = x)).Returns(commandResult);

            // Act
            await controller.Public_AddMessageLogToDatabaseAsync(conversationInfo, messageContent, result);

            // Assert
            Assert.Equal(conversationInfo.BotConversationId, command.FromConversationId);
            Assert.Equal(conversationInfo.ConversationId, command.ToConversationId);
            Assert.Equal(messageContent, command.Content);
            Assert.Equal(result.NotificationId, command.ReferenceId);
            Assert.Equal(result.ErrorCode == 0, command.IsSuccessfull);
            Assert.Equal(result.ErrorMessage, command.ErrorMessage);
        }

        [Fact]
        public async Task AddMessageLogToDatabaseAsync_Exception_CallCorrectlyMethod()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var messageContent = fixture.Create<string>();
            var result = fixture.Create<SendMessageResult>();
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            conversationService.AddMessageLog(Arg.Any<AddMessageLogCommand>()).Returns(Task.FromException<CommandResult>(exception));

            // Act
            await controller.Public_AddMessageLogToDatabaseAsync(conversationInfo, messageContent, result);

            // Assert
            controller.Received().Protected("WriteLogAsync", exception);
        }

        #endregion AddMessageLogToDatabaseAsync

        [Fact]
        public async Task WriteLogAsync_WhenCalled_CallCorrectlyMethod()
        {
            // Arrange
            var exception = fixture.Create<Exception>();
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                        .ToString();
            var controller = GetController();

            // Act
            await controller.Public_WriteLogAsync(exception);

            // Assert
            await logger.Received().ErrorAsync(logMessage);
        }

        #region Helpers

        private MessageControllerTestable GetController(string enableChannelIds = "1,2")
        {
            appSettings = new AppSettings { EnableChannelIds = enableChannelIds };
            var settings = Substitute.For<IOptions<AppSettings>>();
            settings.Value.Returns(appSettings);

            logger = Substitute.For<ILogger>();
            botService = Substitute.For<ITelegramBotService>();
            conversationService = Substitute.For<IConversationService>();
            messageLogService = Substitute.For<IMessageLogService>();
            return Substitute.ForPartsOf<MessageControllerTestable>(settings, logger, conversationService, botService, messageLogService);
        }

        #endregion Helpers
    }
}