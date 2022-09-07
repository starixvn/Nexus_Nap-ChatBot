namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class SendNotificationServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IChatbotService chatbotService;
        private IBaseService baseService;
        private IPushSignalRService pushSignalService;
        private AppSettings appSettings;
        private MonitorChatbot monitorChatBot;
        private ILogService logService;
        private MonitorTelegramBot monitorTelegramBot;

        #region ScanAndSendNotificationAsync

        [Fact]
        public async Task ScanAndSendNotificationAsync_NotificationsAreEmpty_DoNothing()
        {
            // Arrage
            var notifications = new List<NotificationSendMessage>();
            var recipients = new List<NotificationRecipient>();
            var service = GetService();
            service.Protected("GetNotificationPlannedToSendAsync").
                Returns(Task.FromResult<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)>((notifications, recipients)));

            // Act
            await service.ScanAndSendNotificationAsync();

            // Assert
            service.DidNotReceive().Protected("UpdateFailedRecipientStatusAsync", Arg.Any<IEnumerable<NotificationRecipient>>(), false);
            service.DidNotReceive().Protected("SendNotificationToRecipients", notifications, Arg.Any<IEnumerable<NotificationRecipient>>());
        }

        [Fact]
        public async Task ScanAndSendNotificationAsync_HaveNotifications_DoAsExpected()
        {
            // Arrage
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var service = GetService();
            service.Protected("GetNotificationPlannedToSendAsync").
                Returns(Task.FromResult<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)>((notifications, recipients)));

            // Act
            await service.ScanAndSendNotificationAsync();

            // Assert
            service.Received().Protected("UpdateFailedRecipientStatusAsync", Arg.Any<IEnumerable<NotificationRecipient>>(), false);
            service.Received().Protected("SendNotificationToRecipients", notifications, Arg.Any<IEnumerable<NotificationRecipient>>());
        }

        #endregion ScanAndSendNotificationAsync

        [Fact]
        public async Task SendNotificationImmediatelyAsync_WhenCalled_DoAsExpected()
        {
            // Arrage
            var notifiactionId = fixture.Create<int>();
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var criteria = new GetNotificationByIdCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Do<GetNotificationByIdCriteria>(x => criteria = x))
                .Returns((notifications, recipients));

            // Act
            await service.SendNotificationImmediatelyAsync(notifiactionId);

            // Assert
            Assert.Equal(notifiactionId, criteria.NotificationId);

            service.Received().Protected("UpdateFailedRecipientStatusAsync", Arg.Any<IEnumerable<NotificationRecipient>>(), false);
            service.Received().Protected("SendNotificationToRecipients", notifications, Arg.Any<IEnumerable<NotificationRecipient>>());
        }

        #region UpdateFailedRecipientStatusAsync

        [Fact]
        public async Task UpdateFailedRecipientStatusAsync_RecipientsAreEmpty_DoNothing()
        {
            // Arrange
            var recipients = new List<NotificationRecipient>();
            var service = GetService();

            // Act
            await service.Public_UpdateFailedRecipientStatusAsync(recipients);

            // Assert
            service.DidNotReceive().Protected("UpdateSentNotificationStatusAsync", Arg.Any<IEnumerable<UpdateNotificationModel>>());
        }

        [Fact]
        public async Task UpdateFailedRecipientStatusAsync_HaveRecipients_DoAsExpected()
        {
            // Arrange
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 1, RecipientId = 1, ChatGroupId = 1, BotId = 1, ErrCode = 1, ErrorMessage = "error"
                }
            };
            var service = GetService();

            // Act
            await service.Public_UpdateFailedRecipientStatusAsync(recipients);

            // Assert
            service.Received().Protected("UpdateSentNotificationStatusAsync", Arg.Is<IEnumerable<UpdateNotificationModel>>(x =>
                x.Count() == 1 && x.First().NotificationID == 1 && x.First().RecipientID == 1
                && x.First().ConversationGroupID == 1 && x.First().ConversationBotID == 1 && !x.First().Retry
                && !x.First().IsSuccess && x.First().ErrMsg == "Recipient is not mapped"));
        }

        #endregion UpdateFailedRecipientStatusAsync

        #region GetNotificationPlannedToSendAsync

        [Fact]
        public async Task GetNotificationPlannedToSendAsync_DoNotHaveUpdatedNotificationIds_ReturnAsExpectedResult()
        {
            // Arrange
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>())
                .Returns((notifications, recipients));

            // Act
            var result = await service.Public_GetNotificationPlannedToSendAsync();

            // Assert
            Assert.Equal(notifications, result.Item1);
            Assert.Equal(recipients, result.Item2);

            Task.Run(() => pushSignalService.DidNotReceive().PushNotificationUpdateStatus(Arg.Any<string>())).Wait();
        }

        [Fact]
        public async Task GetNotificationPlannedToSendAsync_HaveUpdatedNotificationIds_ReturnAsExpectedResult()
        {
            // Arrange
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var criteria = new GetNotificationsToSendCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Do<GetNotificationsToSendCriteria>(x => criteria = x))
                .Returns((notifications, recipients)).AndDoes(x => criteria.UpdatedNotificationIds = "1");

            // Act
            var result = await service.Public_GetNotificationPlannedToSendAsync();

            // Assert
            Assert.Equal(notifications, result.Item1);
            Assert.Equal(recipients, result.Item2);

            Task.Run(() => pushSignalService.Received().PushNotificationUpdateStatus("1")).Wait();
        }

        #endregion GetNotificationPlannedToSendAsync

        #region SendMultiMessageAsync

        [Fact]
        public async Task SendMultiMessageAsync_Exception_DoAsExpected()
        {
            // Arrange
            byte channelId = 1;
            var messages = fixture.Create<IEnumerable<NotificationMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var originRecipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var exception = fixture.Create<Exception>();
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, messages).Returns(Task.FromException<NotificationMessageResponse>(exception));

            // Act
            await service.Public_SendMultiMessageAsync(channelId, messages, recipients, originRecipients);

            // Assert
            service.Received().Protected("UpdateFailedStatusForBatchAsync", recipients, originRecipients, exception.Message);
            await logService.Received().WriteExceptionSentryErrorAsync(exception);
        }

        [Fact]
        public async Task SendMultiMessageAsync_SendUnsuccessfull_DoAsExpected()
        {
            // Arrange
            byte channelId = 1;
            var messages = fixture.Create<IEnumerable<NotificationMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var originRecipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var response = new NotificationMessageResponse { IsSuccessStatusCode = false, StatusCode = HttpStatusCode.BadRequest };
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, messages).Returns(response);

            // Act
            await service.Public_SendMultiMessageAsync(channelId, messages, recipients, originRecipients);

            // Assert
            service.Received().Protected("UpdateFailedStatusForBatchAsync", recipients, originRecipients, $"Status Code: {response.StatusCode}");
            await logService.Received().WriteMessageSentryErrorAsync($"Send Message fail - Status Code: {response.StatusCode}");
        }

        [Fact]
        public async Task SendMultiMessageAsync_HaveMessageResult_DoAsExpected()
        {
            // Arrange
            byte channelId = 1;
            var messages = fixture.Create<IEnumerable<NotificationMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var originRecipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var response = new NotificationMessageResponse
            {
                IsSuccessStatusCode = true,
                StatusCode = HttpStatusCode.OK,
                NotificationMessageResult = fixture.Create<IEnumerable<SendMessageResult>>()
            };
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, messages).Returns(response);

            // Act
            await service.Public_SendMultiMessageAsync(channelId, messages, recipients, originRecipients);

            // Assert
            service.Received().Protected("UpdateStatusAfterSendNotificationAsync", originRecipients, response.NotificationMessageResult);
        }

        [Fact]
        public async Task SendMultiMessageAsync_DoNotHaveMessageResult_DoAsExpected()
        {
            // Arrange
            byte channelId = 1;
            var messages = fixture.Create<IEnumerable<NotificationMessage>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var originRecipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var response = new NotificationMessageResponse
            {
                IsSuccessStatusCode = true,
                StatusCode = HttpStatusCode.OK
            };
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, messages).Returns(response);

            // Act
            await service.Public_SendMultiMessageAsync(channelId, messages, recipients, originRecipients);

            // Assert
            service.Received().Protected("UpdateFailedStatusForBatchAsync", recipients, originRecipients, "Bot Service Error");
        }

        #endregion SendMultiMessageAsync

        [Fact]
        public async Task UpdateFailedStatusForBatchAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 1, ChatGroupConversationId = "1", RecipientId = 1, ChatGroupId = 1, BotId = 1
                }
            };
            var originRecipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 1, ChatGroupConversationId = "1", RecipientId = 1, ChatGroupId = 1, BotId = 1
                }
            };
            var errorMessage = fixture.Create<string>();
            var service = GetService();

            // Act
            await service.Public_UpdateFailedStatusForBatchAsync(recipients, originRecipients, errorMessage);

            // Assert
            service.Received().Protected("UpdateSentNotificationStatusAsync", Arg.Is<IEnumerable<UpdateNotificationModel>>(x =>
                x.Count() == 1 && x.First().NotificationID == 1 && x.First().RecipientID == 1
                && x.First().ConversationGroupID == 1 && x.First().ConversationBotID == 1 && x.First().Retry
                && !x.First().IsSuccess && x.First().ErrMsg == $"[Internal Error] {errorMessage}"));
        }

        [Fact]
        public async Task UpdateStatusAfterSendNotificationAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient
                {
                    NotificationId = 1, ChatGroupConversationId = "1", RecipientId = 1, ChatGroupId = 1, BotId = 1
                },
                new NotificationRecipient
                {
                    NotificationId = 1, ChatGroupConversationId = "2", RecipientId = 2, ChatGroupId = 2, BotId = 2
                }
            };
            var responseMessages = new List<SendMessageResult>
            {
                new SendMessageResult
                {
                    NotificationId = 1, ConversationId = "1", SentTime = new DateTime(2021, 1, 1), ErrorCode = 0, IsIgnoreRetry = false
                },
                new SendMessageResult
                {
                    NotificationId = 1, ConversationId = "2", SentTime = new DateTime(2021, 1, 1), ErrorCode = 1, ErrorMessage = "error", IsIgnoreRetry = false
                }
            };
            var service = GetService();

            // Act
            await service.Public_UpdateStatusAfterSendNotificationAsync(recipients, responseMessages);

            // Assert
            service.Received().Protected("UpdateSentNotificationStatusAsync", Arg.Is<IEnumerable<UpdateNotificationModel>>(x =>
                x.Count() == 2 && x.First().NotificationID == 1 && x.First().RecipientID == 1 && x.First().SentTime == "2021-01-01 00:00:00"
                && x.First().ConversationGroupID == 1 && x.First().ConversationBotID == 1 && !x.First().Retry
                && x.First().IsSuccess && string.IsNullOrWhiteSpace(x.First().ErrMsg)

                && x.Last().NotificationID == 1 && x.Last().RecipientID == 2 && x.Last().SentTime == "2021-01-01 00:00:00"
                && x.Last().ConversationGroupID == 2 && x.Last().ConversationBotID == 2 && x.Last().Retry
                && !x.Last().IsSuccess && x.Last().ErrMsg == "[Internal Error] error"));
        }

        #region DivideNotificationBatch

        [Fact]
        public void DivideNotificationBatch_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, ChannelId = 1, Content = "**content**" }
            };
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient { NotificationId = 1, ChatGroupConversationId = "1", BotConversationId = "1" },
                new NotificationRecipient { NotificationId = 1, ChatGroupConversationId = "2", BotConversationId = "2" }
            };
            var lastrecipients = fixture.Create<IEnumerable<ConversationInfo>>();
            var service = GetService();
            service.Protected("AddMonitorRecipient", Arg.Any<IEnumerable<ConversationInfo>>()).Returns(lastrecipients);

            // Arrange
            var result = service.Public_DivideNotificationBatch(1, notifications, recipients);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Collection(result.First().Item1, item =>
            {
                Assert.Equal(1, item.NotificationId);
                Assert.Equal(1, item.ChannelId);
                Assert.Equal("**content**", item.Message);

                Assert.Single(item.Recipients);
                Assert.Collection(item.Recipients, recipient =>
                {
                    Assert.Equal("1", recipient.BotConversationId);
                    Assert.Equal("1", recipient.ConversationId);
                });
            });

            Assert.Collection(result.First().Item2, item =>
            {
                Assert.Equal(1, item.NotificationId);
                Assert.Equal("1", item.BotConversationId);
                Assert.Equal("1", item.ChatGroupConversationId);
            });

            Assert.Collection(result.Last().Item1, item =>
            {
                Assert.Equal(1, item.NotificationId);
                Assert.Equal(1, item.ChannelId);
                Assert.Equal("**content**", item.Message);

                Assert.Equal(lastrecipients, item.Recipients);
            });

            Assert.Collection(result.Last().Item2, item =>
            {
                Assert.Equal(1, item.NotificationId);
                Assert.Equal("2", item.BotConversationId);
                Assert.Equal("2", item.ChatGroupConversationId);
            });
        }

        #endregion DivideNotificationBatch

        #region AddMonitorRecipient

        [Fact]
        public void AddMonitorRecipient_DisableMonitor_ReturnAsExpectedResult()
        {
            // Arrange
            var conversations = new List<ConversationInfo>();
            var service = GetService(enableMonitorSkypeBot: false);

            // Act
            var result = service.Public_AddMonitorRecipient(conversations);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddMonitorRecipient_EnableMonitor_ReturnAsExpectedResult()
        {
            // Arrange
            var conversations = new List<ConversationInfo>();
            var service = GetService();

            // Act
            var result = service.Public_AddMonitorRecipient(conversations);

            // Assert
            Assert.Single(result);
            Assert.Collection(result, item =>
            {
                Assert.Equal(monitorChatBot.ConversationId, item.ConversationId);
                Assert.Equal(monitorChatBot.ChannelId, item.ChannelId);
                Assert.Equal(monitorChatBot.ServiceUrl, item.ServiceUrl);
                Assert.Equal(monitorChatBot.BotConversationId, item.BotConversationId);
                Assert.Equal(monitorChatBot.BotName, item.BotName);
            });
        }

        #endregion AddMonitorRecipient

        #region AddMonitorRecipientForTelegram

        [Fact]
        public void AddMonitorRecipientForTelegram_DisableMonitor_ReturnAsExpectedResult()
        {
            // Arrange
            var conversations = new List<ConversationInfo>();
            var service = GetService(enableMonitorTelegramBot: false);

            // Act
            var result = service.Public_AddMonitorRecipientForTelegram(conversations);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddMonitorRecipientForTelegram_EnableMonitor_ReturnAsExpectedResult()
        {
            // Arrange
            var conversations = new List<ConversationInfo>();
            var service = GetService();

            // Act
            var result = service.Public_AddMonitorRecipientForTelegram(conversations);

            // Assert
            Assert.Single(result);
            Assert.Collection(result, item =>
            {
                Assert.Equal(monitorTelegramBot.ChatId.ToString(), item.ConversationId);
                Assert.Equal(monitorTelegramBot.BotId.ToString(), item.BotConversationId);
            });
        }

        #endregion AddMonitorRecipientForTelegram

        #region UpdateSentNotificationStatusAsync

        [Fact]
        public async Task UpdateSentNotificationStatusAsync_MessagesAreEmpty_DoNothing()
        {
            // Arrange
            var messages = new List<UpdateNotificationModel>();
            var service = GetService();

            // Act
            await service.Public_UpdateSentNotificationStatusAsync(messages);

            // Assert
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<UpdateRecipientSendNotifiactionStatusCommand>());
        }

        [Fact]
        public async Task UpdateSentNotificationStatusAsync_UpdatedNotificationIds_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<UpdateNotificationModel>>();
            var commandResult = fixture.Create<CommandResult>();
            var command = new UpdateRecipientSendNotifiactionStatusCommand();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateRecipientSendNotifiactionStatusCommand>(x => command = x))
                .Returns(commandResult).AndDoes(x => command.UpdatedNotificationIds = "1");

            // Act
            await service.Public_UpdateSentNotificationStatusAsync(messages);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(messages), command.SentStatus);
            Task.Run(() => pushSignalService.Received().PushNotificationUpdateStatus("1")).Wait();
            Task.Run(() => pushSignalService.DidNotReceive().PushErrorNotification()).Wait();
        }

        [Fact]
        public async Task UpdateSentNotificationStatusAsync_HaveErrNotification_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<UpdateNotificationModel>>();
            var commandResult = fixture.Create<CommandResult>();
            var command = new UpdateRecipientSendNotifiactionStatusCommand();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateRecipientSendNotifiactionStatusCommand>(x => command = x))
                .Returns(commandResult).AndDoes(x => command.IsHaveErrNotification = true);

            // Act
            await service.Public_UpdateSentNotificationStatusAsync(messages);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(messages), command.SentStatus);
            Task.Run(() => pushSignalService.DidNotReceive().PushNotificationUpdateStatus(Arg.Any<string>())).Wait();
            Task.Run(() => pushSignalService.Received().PushErrorNotification()).Wait();
        }

        #endregion UpdateSentNotificationStatusAsync

        #region Helpers

        private SendNotificationServiceTestable GetService(
            bool enableMonitorSkypeBot = true,
            bool enableMonitorTelegramBot = true)
        {
            chatbotService = Substitute.For<IChatbotService>();
            baseService = Substitute.For<IBaseService>();
            pushSignalService = Substitute.For<IPushSignalRService>();
            logService = Substitute.For<ILogService>();

            appSettings = new AppSettings
            {
                ItemEachBatchSendNotification = 1,
            };
            var appSettingOptions = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettingOptions.Value.Returns(appSettings);

            monitorChatBot = fixture.Create<MonitorChatbot>();
            monitorChatBot.IsMonitoring = enableMonitorSkypeBot;
            var monitorChatBotOptions = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
            monitorChatBotOptions.Value.Returns(monitorChatBot);

            monitorTelegramBot = fixture.Create<MonitorTelegramBot>();
            monitorTelegramBot.IsMonitoring = enableMonitorTelegramBot;
            var monitorTelegramBotOptions = Substitute.For<IOptionsSnapshot<MonitorTelegramBot>>();
            monitorTelegramBotOptions.Value.Returns(monitorTelegramBot);

            return Substitute.For<SendNotificationServiceTestable>(
                chatbotService,
                baseService,
                pushSignalService,
                logService,
                appSettingOptions,
                monitorChatBotOptions,
                monitorTelegramBotOptions);
        }

        #endregion Helpers
    }
}