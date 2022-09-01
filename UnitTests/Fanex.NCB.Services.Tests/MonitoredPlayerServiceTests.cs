namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Criteria.MonitoredPlayer;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.MonitoredPlayer;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class MonitoredPlayerServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private IChatbotService chatbotService;
        private IPushSignalService pushSignalService;
        private ILogService logService;

        [Fact]
        public async Task SendMessageMonitoredPlayerAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            var criteria = fixture.Create<GetMsgSettingMonitoredPlayerCriteria>();
            MessageSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Do<GetMsgSettingMonitoredPlayerCriteria>(x => criteria = x)).Returns(setting);

            // Act
            await service.SendMessageMonitoredPlayerAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<MonitoredPlayerInfo, short>(Arg.Any<GetMonitoredPlayerInfoCriteria>());
        }

        [Fact]
        public async Task SendMessageMonitoredPlayerAsync_NoDataToImport_DoNothing()
        {
            // Arrange
            var setting = fixture.Create<MessageSetting>();
            IEnumerable<MonitoredPlayerInfo> monitoredPlayerInfo = new List<MonitoredPlayerInfo>();

            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Any<GetMsgSettingMonitoredPlayerCriteria>()).Returns(setting);
            baseService.GetDataAsync<MonitoredPlayerInfo>(Arg.Any<GetMonitoredPlayerInfoCriteria>()).Returns(monitoredPlayerInfo);

            // Act
            await service.SendMessageMonitoredPlayerAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportFromMonitoredPlayerCriteria>());
        }

        [Fact]
        public async Task SendMessageMonitoredPlayerAsync_HasDataToImport_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<MessageSetting>();
            var monitoredPlayerInfo = fixture.Create<IEnumerable<MonitoredPlayerInfo>>();

            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Any<GetMsgSettingMonitoredPlayerCriteria>()).Returns(setting);
            baseService.GetDataAsync<MonitoredPlayerInfo>(Arg.Any<GetMonitoredPlayerInfoCriteria>()).Returns(monitoredPlayerInfo);

            // Act
            await service.SendMessageMonitoredPlayerAsync();

            // Assert
            await baseService.Received().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportFromMonitoredPlayerCriteria>());
        }

        [Fact]
        public async Task GetMessageSettingAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetMsgSettingMonitoredPlayerCriteria>();
            var setting = fixture.Create<MessageSetting>();

            baseService.GetDataEntityAsync<MessageSetting>(Arg.Do<GetMsgSettingMonitoredPlayerCriteria>(x => criteria = x)).Returns(setting);

            //Act
            var actual = await service.GetMessageSettingAsync();

            //Assert
            Assert.Equal(setting, actual);
        }

        [Fact]
        public async Task GetMonitoredPlayerAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var setting = fixture.Create<MessageSetting>();
            var criteria = fixture.Create<GetMonitoredPlayerInfoCriteria>();
            var monitoredPlayerInfo = fixture.Create<IEnumerable<MonitoredPlayerInfo>>();

            baseService.GetDataAsync<MonitoredPlayerInfo>(Arg.Any<GetMonitoredPlayerInfoCriteria>()).Returns(monitoredPlayerInfo);

            //Act
            var actual = await service.GetMonitoredPlayerAsync(setting);

            //Assert
            Assert.Equal(monitoredPlayerInfo, actual.Item1);
        }

        [Fact]
        public async Task ImportMessageFromMonitoredPlayerAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<MonitoredPlayerInfo>>();
            var serverName = "TestDb2053";
            var lastDataRow = "<root/>";
            var pLastLogId = 1;
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var tuple = (recipients, notificationMessages);
            var criteria = new ImportFromMonitoredPlayerCriteria()
            {
                Message = JsonConvert.SerializeObject(messages),
                ServerName = serverName,
                LastDataRow = lastDataRow,
                PLastLogId = pLastLogId
            };

            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportFromMonitoredPlayerCriteria>(x => criteria = x)).Returns(tuple);
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);

            // Act
            var result = await service.ImportMessageFromMonitoredPlayerAsync(messages, serverName, lastDataRow, pLastLogId);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(messages), criteria.Message);
            Assert.Equal(criteria.ErrCode, result.Item1.Code);
            Assert.Equal(criteria.ErrMsg, result.Item1.Message);

            Assert.Equal(notifications, result.Item2);
        }

        #region OnAfterImportDataAsync

        [Fact]
        public async Task OnAfterImportDataAsync_IsSuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.Public_OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.DidNotReceive().NotifyToChatGroupAsync(Arg.Any<string>());

            await pushSignalService.Received().OnInsertNotificationAsync(notificationIds);
            service.Received().Protected("SendMessageByChannelAsync", (byte)1, notifications);
            service.Received().Protected("SendMessageByChannelAsync", (byte)2, notifications);
        }

        [Fact]
        public async Task OnAfterImportDataAsync_IsUnsuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 1, Message = "error" };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.Public_OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.Received().NotifyToChatGroupAsync("error");

            await pushSignalService.DidNotReceive().OnInsertNotificationAsync(notificationIds);
            service.DidNotReceive().Protected("SendMessageByChannelAsync", (byte)1, notifications);
            service.DidNotReceive().Protected("SendMessageByChannelAsync", (byte)2, notifications);
        }

        #endregion OnAfterImportDataAsync

        #region SendMessageByChannelAsync

        [Fact]
        public async Task SendMessageByChannelAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var channelId = fixture.Create<byte>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var service = GetService();

            // Act
            await service.Public_SendMessageByChannelAsync(channelId, notifications);

            // Assert
            await chatbotService.Received().SendMultiMessageAsync(channelId, Arg.Any<IEnumerable<NotificationMessage>>());
        }

        [Fact]
        public async Task SendMessageByChannelAsync_Exception_DoAsExpected()
        {
            // Arrange
            var channelId = fixture.Create<byte>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var exception = fixture.Create<Exception>();
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, Arg.Any<IEnumerable<NotificationMessage>>())
                .Returns(Task.FromException<NotificationMessageResponse>(exception));

            // Act
            await service.Public_SendMessageByChannelAsync(channelId, notifications);

            // Assert
            logService.Received().WriteLogFileError(exception.Message);
        }

        #endregion SendMessageByChannelAsync

        #region Helpers

        private MonitoredPlayerServiceTestable GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            pushSignalService = Substitute.For<IPushSignalService>();
            logService = Substitute.For<ILogService>();

            return Substitute.For<MonitoredPlayerServiceTestable>(baseService, chatbotService, pushSignalService, logService);
        }

        #endregion Helpers
    }
}
