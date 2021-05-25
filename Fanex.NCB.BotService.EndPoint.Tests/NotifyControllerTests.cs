namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Schema;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class NotifyControllerTests
    {
        private IAdapterIntegration adapter;
        private IMessageLogService messageLogService;
        private IConversationService conversationService;
        private ILogger logger;

        [Obsolete]
        private BotConfiguration botConfig;

        private CheckDuplicatedMessageLogSetting checkDuplicatedMessageLogSetting;

        [Fact]
        [Obsolete]
        public async Task Send_InValidBotName_SendContinueWithDefaultAppId()
        {
            var controller = GetController();
            var botName = string.Empty;
            var conversationId = string.Empty;
            var message = string.Empty;

            var conversationGroup = new ConversationGroup
            {
                BotConversationId = string.Empty,
                BotName = string.Empty,
                ConversationId = string.Empty,
                GroupName = string.Empty,
                ChannelId = string.Empty,
                ServiceUrl = string.Empty
            };

            conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);

            //Actual
            var actual = await controller.Send(botName, conversationId, message);

            //Assert
            Assert.IsType<OkObjectResult>(actual);

            await adapter.Received().ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        [Obsolete]
        public async Task Send_ValidBotName_ReturOkObjectResult()
        {
            var controller = GetController();

            var botName = string.Empty;
            var conversationId = string.Empty;
            var message = string.Empty;

            var endpointService = new EndpointService
            {
                AppId = "1",
                AppPassword = "1",
                Name = botName
            };

            botConfig.Services = new List<ConnectedService>
            {
                endpointService
            };

            var conversationGroup = new ConversationGroup();
            conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);

            //Actual
            var actual = await controller.Send(botName, conversationId, message);

            //Assert
            Assert.IsType<OkObjectResult>(actual);

            await adapter.Received().ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        [Obsolete]
        public async Task Send_GetGroupConversationThrowException_ReturnExpectedErrorMessage()
        {
            var controller = GetController();

            var botName = string.Empty;
            var conversationId = string.Empty;
            var message = string.Empty;

            var conversationGroup = new ConversationGroup
            {
                BotConversationId = string.Empty,
                BotName = string.Empty,
                ConversationId = string.Empty,
                GroupName = string.Empty,
                ChannelId = string.Empty,
                ServiceUrl = string.Empty
            };

            conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);

            adapter.ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>()).Throws(new Exception("error msg"));

            //Actual
            var actual = await controller.Send(botName, conversationId, message);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Equal("error msg", actual.Value);
        }

        [Fact]
        [Obsolete]
        public async Task SendMulti_NotificationListIsNull_ReturnObjectResultWithEmptyList()
        {
            var controller = GetController();

            //Actual
            var actual = await controller.SendMulti(null);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Empty((List<SendMessageResult>)actual.Value);
        }

        [Fact]
        [Obsolete]
        public async Task SendMulti_NotificationListIsEmpty_ReturnObjectResultWithEmptyList()
        {
            var controller = GetController();
            var notificationMessage = new List<NotificationMessage> { new NotificationMessage { NotificationId = 1, Recipients = new List<ConversationInfo>() } };

            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Empty((List<SendMessageResult>)actual.Value);
        }

        [Fact]
        [Obsolete]
        public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithSuccessList()
        {
            var controller = GetController();

            var conversationGroup = new ConversationGroup();

            conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);

            var notificationMessage = new List<NotificationMessage>
            {
                new NotificationMessage {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo
                        {
                            ConversationId = "ConversationId"
                        }
                    }
                }
            };

            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Single((List<SendMessageResult>)actual.Value);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
            Assert.Equal("ConversationId", ((List<SendMessageResult>)actual.Value)[0].ConversationId);
            Assert.Equal(0, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
            Assert.Null(((List<SendMessageResult>)actual.Value)[0].ErrorMessage);

            await adapter.Received().ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        [Obsolete]
        public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithErrorList()
        {
            var controller = GetController();

            var conversationGroup = new ConversationGroup
            {
                BotConversationId = string.Empty,
                BotName = string.Empty,
                ConversationId = string.Empty,
                GroupName = string.Empty,
                ChannelId = string.Empty,
                ServiceUrl = string.Empty
            };

            conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);

            adapter.ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>()).Throws(new Exception("error msg"));

            var notificationMessage = new List<NotificationMessage>
            {
                new NotificationMessage {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo()
                    }
                }
            };

            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Single((List<SendMessageResult>)actual.Value);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
            Assert.Equal("error msg", ((List<SendMessageResult>)actual.Value)[0].ErrorMessage);

            await adapter.Received().ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
                Arg.Any<CancellationToken>());
        }

        [Obsolete]
        private NotifyController GetController()
        {
            adapter = Substitute.For<IAdapterIntegration>();
            messageLogService = Substitute.For<IMessageLogService>();
            botConfig = Substitute.For<BotConfiguration>();
            checkDuplicatedMessageLogSetting = new CheckDuplicatedMessageLogSetting { IsEnabled = true, TimeSpanInMinutes = 1, ExceptionConversatiionIDs = ["ABC"] };
            conversationService = Substitute.For<IConversationService>();
            logger = Substitute.For<ILogger>();
            return new NotifyController(adapter, messageLogService, botConfig, conversationService, checkDuplicatedMessageLogSetting, logger);
        }
    }
}