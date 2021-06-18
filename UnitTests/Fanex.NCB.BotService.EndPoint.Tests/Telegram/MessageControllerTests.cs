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
    using global::Telegram.Bot.Types;
    using Microsoft.AspNetCore.Mvc;
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

        #region SendMessage

        [Fact]
        public async Task SendMessage_DisableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var controller = GetController("1");

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", okResult.Value);

            await botService.DidNotReceive().SendTextMessageAsync(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<string>());
        }

        [Fact]
        public async Task SendMessage_EnableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo
            {
                BotConversationId = "1",
                ConversationId = "1",
                Message = "message"
            };
            var controller = GetController();

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty((string)okResult.Value);

            await botService.Received().SendTextMessageAsync(1, 1, conversationInfo.Message);
        }

        [Fact]
        public async Task SendMessage_Exception_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo
            {
                BotConversationId = "1",
                ConversationId = "1",
                Message = "message"
            };
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            botService.SendTextMessageAsync(1, 1, conversationInfo.Message).Returns(Task.FromException<Message>(exception));

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(exception.Message, okResult.Value);

            controller.Received().Protected("WriteLogAsync", exception);
        }

        #endregion SendMessage

        #region SendMessageAsync
        #endregion

        #region SendTextMessageAsync

        [Fact]
        public async Task SendTextMessageAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var botId = fixture.Create<long>();
            var chatId = fixture.Create<long>();
            var notificationId = fixture.Create<int>();
            var messageContent = fixture.Create<string>();
            var controller = GetController();

            // Act
            var result = await controller.Public_SendTextMessageAsync(botId, chatId, notificationId, messageContent);

            // Assert
            Assert.Equal(chatId.ToString(), result.ConversationId);
            Assert.Equal(notificationId, result.NotificationId);
            Assert.Equal(0, result.ErrorCode);
            Assert.Null(result.ErrorMessage);
            Assert.False(result.IsIgnoreRetry);

            await botService.Received().SendTextMessageAsync(botId, chatId, messageContent);
        }

        [Fact]
        public async Task SendTextMessageAsync_Exception_ReturnAsExpectedResult()
        {
            // Arrange
            var botId = fixture.Create<long>();
            var chatId = fixture.Create<long>();
            var notificationId = fixture.Create<int>();
            var messageContent = fixture.Create<string>();
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            botService.SendTextMessageAsync(botId, chatId, messageContent).Returns(Task.FromException<Message>(exception));

            // Act
            var result = await controller.Public_SendTextMessageAsync(botId, chatId, notificationId, messageContent);

            // Assert
            Assert.Equal(chatId.ToString(), result.ConversationId);
            Assert.Equal(notificationId, result.NotificationId);
            Assert.Equal(1, result.ErrorCode);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.False(result.IsIgnoreRetry);
        }

        #endregion SendTextMessageAsync

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