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
        private IPushSignalService pushSignalService;
        private AppSettings appSettings;
        private MonitorChatbot monitorChatBot;
        private ILogService logService;
        private MonitorTelegramBot monitorTelegramBot;

        ////[Fact]
        ////public async Task ScanAndSendNotification_ScanEmptyNoti_NotUpdateStatusAndSendNoti()
        ////{
        ////    var notifications = new List<NotificationSendMessage>();
        ////    var recipients = new List<NotificationRecipient>();
        ////    var emptyListNoti = new ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>>(notifications, recipients);
        ////    var service = GetService();
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(emptyListNoti);

        ////    //Actual
        ////    await service.ScanAndSendNotification();

        ////    //Assert
        ////    await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////    await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        ////}

        ////[Fact]
        ////public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendFailed()
        ////{
        ////    var notifications = FakeData();
        ////    var service = GetService();
        ////    var response = FakeResponse(false);
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);
        ////    chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);

        ////    //Actual
        ////    await service.ScanAndSendNotification();

        ////    //Assert
        ////    await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////    await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        ////}

        ////[Fact]
        ////public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        ////{
        ////    var notifications = FakeData();
        ////    var response = FakeResponse(true);
        ////    var service = GetService();
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);
        ////    chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
        ////    //Actual
        ////    await service.ScanAndSendNotification();

        ////    //Assert
        ////    await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////}

        ////[Fact]
        ////public async Task ScanAndSendNotification_ScanHasNoti_ChatbotServiceThrowExceptions()
        ////{
        ////    var notifications = FakeData();
        ////    var service = GetService();
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);

        ////    chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Throws(new Exception());
        ////    await service.ScanAndSendNotification();
        ////    //Assert
        ////    await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        ////}

        ////[Fact]
        ////public async Task SendNotificationImmediately_IsCreating_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        ////{
        ////    var notifications = FakeData();
        ////    var response = FakeResponse(true);
        ////    var service = GetService();
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationByIdCriteria>()).Returns(notifications);
        ////    chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
        ////    //Actual
        ////    await service.SendNotificationImmediately(1);
        ////    PushSignalNewNotification("1");
        ////    //Assert
        ////    await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////    await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        ////}

        ////[Fact]
        ////public async Task SendNotificationImmediately_NotIsCreating_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        ////{
        ////    var notifications = FakeData();
        ////    var response = FakeResponse(true);
        ////    var service = GetService();
        ////    baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationByIdCriteria>()).Returns(notifications);
        ////    chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
        ////    //Actual
        ////    await service.SendNotificationImmediately(1);

        ////    PushSignalUpdateNotiStatus("1");
        ////    //Assert
        ////    await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////    await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        ////}

        private void PushSignalNewNotification(string notificationIds)
        {
            Task.Run(() => pushSignalService.Received().OnInsertNotificationAsync(notificationIds));
        }

        private void PushSignalUpdateNotiStatus(string notificationIds)
        {
            Task.Run(() => pushSignalService.Received().OnUpdateNotificationStatusAsync(notificationIds));
        }

        #region Helper

        private static ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>> FakeData()
        {
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, Content = "Text"} ,
                new NotificationSendMessage { NotificationId = 2, Content = "Text 2" }
            };

            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient {NotificationId = 1, ErrCode = 0, ChatGroupConversationId = "ABC"},
                new NotificationRecipient {NotificationId = 2, ErrCode = 0, ChatGroupConversationId = "CDE"},
                new NotificationRecipient {NotificationId = 2, ErrCode = 1, ChatGroupConversationId = "FGH"}
            };
            return new ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>>(notifications, recipients);
        }

        private static NotificationMessageResponse FakeResponse(bool isSuccess)
        {
            var result = new List<SendMessageResult>
            {
                new SendMessageResult { NotificationId = 1, ConversationId = "1", ErrorCode = 0, ErrorMessage = "", SentTime = DateTime.Now }
            };
            return new NotificationMessageResponse
            {
                IsSuccessStatusCode = isSuccess,
                StatusCode = isSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
                NotificationMessageResult = new List<SendMessageResult>
                {
                    new SendMessageResult
                    {
                        NotificationId = 1, ConversationId = "ABC", ErrorCode = 0, ErrorMessage = "", SentTime = new DateTime(2020, 10, 1)
                    },
                    new SendMessageResult
                    {
                        NotificationId = 2, ConversationId = "DEF", ErrorCode = 1, ErrorMessage = "Error", SentTime = new DateTime(2020, 10, 1)
                    }
                }
            };
        }

        #region DivideNotificationBatch

        [Fact]
        public void DivideNotificationBatch_ChannelIsSkype_ReturnAsExpectedResult()
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

        [Fact]
        public void DivideNotificationBatch_ChannelIsTelegram_ReturnAsExpectedResult()
        {
            // Arrange
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, ChannelId = 2, Content = "**content**" }
            };
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient { NotificationId = 1, ChatGroupConversationId = "1", BotConversationId = "1" },
                new NotificationRecipient { NotificationId = 1, ChatGroupConversationId = "2", BotConversationId = "2" }
            };
            var lastrecipients = fixture.Create<IEnumerable<ConversationInfo>>();
            var service = GetService();
            service.Protected("AddMonitorRecipientForTelegram", Arg.Any<IEnumerable<ConversationInfo>>()).Returns(lastrecipients);

            // Arrange
            var result = service.Public_DivideNotificationBatch(2, notifications, recipients);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Collection(result.First().Item1, item =>
            {
                Assert.Equal(1, item.NotificationId);
                Assert.Equal(2, item.ChannelId);
                Assert.Equal("*content*", item.Message);

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
                Assert.Equal(2, item.ChannelId);
                Assert.Equal("*content*", item.Message);

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

        ////[Fact]
        ////public async Task UpdateSentNotificationStatusAsync_UpdatedNotificationIds_ExpectedCallCorrectMethod()
        ////{
        ////    // Arrange
        ////    var messages = fixture.Create<IEnumerable<UpdateNotificationModel>>();
        ////    var commandResult = fixture.Create<CommandResult>();
        ////    var command = new UpdateRecipientSendNotifiactionStatusCommand();
        ////    var service = GetService();
        ////    baseService.ExecuteNonQueryAsync(Arg.Do<UpdateRecipientSendNotifiactionStatusCommand>(x => command = x))
        ////        .Returns(commandResult).AndDoes(x => command.UpdatedNotificationIds = "1");

        ////    // Act
        ////    await service.Public_UpdateSentNotificationStatusAsync(messages);

        ////    // Assert
        ////    Assert.Equal(JsonConvert.SerializeObject(messages), command.SentStatus);
        ////    await pushSignalService.Received().OnUpdateNotificationStatusAsync("1");
        ////    await pushSignalService.DidNotReceive().OnHasNewErrNotificationAsync();
        ////}

        ////[Fact]
        ////public async Task UpdateSentNotificationStatusAsync_HaveErrNotification_ExpectedCallCorrectMethod()
        ////{
        ////    // Arrange
        ////    var messages = fixture.Create<IEnumerable<UpdateNotificationModel>>();
        ////    var commandResult = fixture.Create<CommandResult>();
        ////    var command = new UpdateRecipientSendNotifiactionStatusCommand();
        ////    var service = GetService();
        ////    baseService.ExecuteNonQueryAsync(Arg.Do<UpdateRecipientSendNotifiactionStatusCommand>(x => command = x))
        ////        .Returns(commandResult).AndDoes(x => command.IsHaveErrNotification = true);

        ////    // Act
        ////    await service.Public_UpdateSentNotificationStatusAsync(messages);

        ////    // Assert
        ////    Assert.Equal(JsonConvert.SerializeObject(messages), command.SentStatus);
        ////    await pushSignalService.DidNotReceive().OnUpdateNotificationStatusAsync(Arg.Any<string>());
        ////    await pushSignalService.Received().OnHasNewErrNotificationAsync();
        ////}

        #endregion UpdateSentNotificationStatusAsync

        private SendNotificationServiceTestable GetService(
            bool enableMonitorSkypeBot = true,
            bool enableMonitorTelegramBot = true)
        {
            chatbotService = Substitute.For<IChatbotService>();
            baseService = Substitute.For<IBaseService>();
            pushSignalService = Substitute.For<IPushSignalService>();
            logService = Substitute.For<ILogService>();

            appSettings = fixture.Create<AppSettings>();
            appSettings.ItemEachBatchSendNotification = 1;
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

        #endregion Helper
    }
}