namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class NotifyControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private AppSettings appSettings;
        private IMessageLogService messageLogService;
        private IConversationService conversationService;
        private ILogger<NotifyController> filelogger;
        private Logging.ILogger logger;
        private ISkypeBotService skypeBotService;

        [Fact]
        public async Task Test_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.Test();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("ok", okResult.Value);

            await logger.Received().ErrorAsync("test sentry");
        }

        #region Send

        [Fact]
        public async Task Send_DisableSkype_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo();
            var controller = GetController("2");

            // Act
            var result = await controller.Send(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", okResult.Value);
        }

        [Fact]
        public async Task Send_EnableSkype_ReturnAsExpectedResult()
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
            var result = await controller.Send(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty((string)okResult.Value);

            await skypeBotService.Received().SendMessageAsync(conversationInfo);
        }

        #endregion Send

        [Fact]
        public async Task SendMulti_NotificationListIsNull_ReturnObjectResultWithEmptyList()
        {
            var controller = GetController();

            //Actual
            var actual = await controller.SendMulti(null);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Empty((List<SendMessageResult>)actual.Value);
        }

        private NotifyController GetController(string enableChannelIds = "1,2")
        {
            appSettings = new AppSettings { EnableChannelIds = enableChannelIds };
            var appSettingOptions = Substitute.For<IOptions<AppSettings>>();
            appSettingOptions.Value.Returns(appSettings);

            filelogger = Substitute.For<ILogger<NotifyController>>();
            logger = Substitute.For<Logging.ILogger>();
            messageLogService = Substitute.For<IMessageLogService>();
            conversationService = Substitute.For<IConversationService>();
            skypeBotService = Substitute.For<ISkypeBotService>();
            return new NotifyController(appSettingOptions, filelogger, logger, messageLogService, conversationService, skypeBotService);
        }
    }
}