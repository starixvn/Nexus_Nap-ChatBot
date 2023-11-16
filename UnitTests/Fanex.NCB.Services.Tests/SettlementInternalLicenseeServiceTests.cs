namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SettlementInternalCSD;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.Settlement;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class SettlementInternalLicenseeServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private ICommonService commonService;
        private IChatbotService chatbotService;
        private IPushSignalRService pushSignalService;
        private ILogService logService;

        [Fact]
        public async Task GetServcieSetting_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var service = GetService();
            baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(setting);

            // Act
            var actual = await service.GetServcieSetting("inServiceName");

            // Assert
            Assert.Equal(setting, actual);
        }

        #region Settlement Internal CSD

        [Fact]
        public async Task SettlementInternalCSDAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            ServiceSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(setting);

            // Act
            await service.SettlementInternalCSDAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(Arg.Any<GetMessageSettleInternalCSDCriteria>());
        }

        [Fact]
        public async Task SettlementInternalCSDAsync_HasDataToImport_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var tuple = (messageSettlement, messageSettlementInfo);
            var service = GetService();

            baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(setting);
            baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(Arg.Any<GetMessageSettleInternalCSDCriteria>()).Returns(tuple);

            // Act
            await service.SettlementInternalCSDAsync();

            // Assert
            service.Received().Protected("GetMessageAndImportToChatbotAsync", messageSettlement, messageSettlementInfo);
        }

        //[Fact]
        //public async Task GetMessageAndImportToChatbotAsync_HasDataToImport_DoAsExpected()
        //{
        //    // Arrange
        //    var setting = fixture.Create<ServiceSetting>();
        //    var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
        //    var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
        //    var tuple = (messageSettlement, messageSettlementInfo);
        //    var service = GetService();

        //    baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(setting);
        //    baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(Arg.Any<GetMessageSettleInternalCSDCriteria>()).Returns(tuple);

        //    // Act
        //    await service.GetMessageAndImportToChatbotAsync(messageSettlement, messageSettlementInfo);

        //    // Assert
        //    service.Received().Protected("GetMessageAndImportToChatbotAsync", messageSettlement, messageSettlementInfo);
        //}

        #endregion  Settlement Internal CSD

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

            await pushSignalService.Received().PushNewNotification(notificationIds);
            service.Received().Protected("SendMessageByChannelAsync", (byte)1, notifications);
            service.Received().Protected("SendMessageByChannelAsync", (byte)2, notifications);
        }

        [Fact]
        public async Task OnAfterImportDataAsync_IsUnsuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 1, Message = "error" };
            var notifications = fixture.CreateMany<NotificationMessage>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.Public_OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.Received().NotifyToChatGroupAsync("error");

            await pushSignalService.DidNotReceive().PushNewNotification(notificationIds);
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

        private SettlementInternalLicenseeServiceTestable GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            chatbotService = Substitute.For<IChatbotService>();
            pushSignalService = Substitute.For<IPushSignalRService>();
            logService = Substitute.For<ILogService>();

            return Substitute.For<SettlementInternalLicenseeServiceTestable>(baseService, pushSignalService, chatbotService, logService, commonService);
        }

        #endregion Helpers
    }
}
