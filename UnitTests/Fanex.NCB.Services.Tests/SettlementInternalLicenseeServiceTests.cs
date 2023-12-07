namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Criteria.SettlementInternalCSD;
    using Fanex.NCB.Criteria.SettlementLicensee;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.Settlement;
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

        #region Settlement Internal CSD

        [Fact]
        public async Task SettlementInternalCSDAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var tuple = (messageSettlement, messageSettlementInfo);
            var service = GetService();

            commonService.GetServiceSetting(Arg.Any<string>()).Returns(setting);
            baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(Arg.Any<GetMessageSettleInternalCSDCriteria>()).Returns(tuple);

            // Act
            await service.SettlementInternalCSDAsync();

            // Assert
            service.Received().Protected("GetMessageAndImportToChatbotAsync", messageSettlement, messageSettlementInfo, 0);
        }

        [Fact]
        public async Task GetMessageAndImportToChatbotAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var messages = fixture.Create<IEnumerable<MessageSettlementHeader>>();
            var messageDetails = fixture.Create<IEnumerable<MessageTarget>>();
            var maxMsgTargetId = fixture.Create<int>();


            var service = GetService();

            service.Protected("GetEventInfoAsync", messageSettlement, messageSettlementInfo)
                .Returns(Task.FromResult<(IEnumerable<MessageSettlementHeader>, IEnumerable<MessageTarget>)>((messages, messageDetails)));

            // Act
            await service.Public_GetMessageAndImportToChatbotAsync(messageSettlement, messageSettlementInfo, maxMsgTargetId);

            // Assert
            service.Received().Protected("ImportSettlementInternalCSDToChatbotDBAsync", messages, messageDetails, maxMsgTargetId);
        }

        [Fact]
        public async Task GetEventInfoAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var messageSettlementHeader = fixture.Create<IEnumerable<MessageSettlementHeader>>();
            var messageTarget = fixture.Create<IEnumerable<MessageTarget>>();
            var criteria = fixture.Create<GetEventSettleInternalCSDCriteria>();
            var tuple = (messageSettlementHeader, messageTarget);
            var service = GetService();

            baseService.FetchMultipleAsync<MessageSettlementHeader, MessageTarget>(Arg.Do<GetEventSettleInternalCSDCriteria>(x => criteria = x)).Returns(tuple);

            // Act
            var result = await service.Public_GetEventInfoAsync(messageSettlement, messageSettlementInfo);

            // Assert
            Assert.Equal(tuple.messageSettlementHeader, result.Item1);
            Assert.Equal(tuple.messageTarget, result.Item2);
        }

        [Fact]
        public async Task ImportSettlementInternalCSDToChatbotDBAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messageSettlementHeader = fixture.Create<IEnumerable<MessageSettlementHeader>>();
            var messageTarget = fixture.Create<IEnumerable<MessageTarget>>();
            var criteria = fixture.Create<ImportSettleInternalCSDCommand>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var maxMsgTargetId = fixture.Create<int>();
            var tuple = (recipients, notificationMessages);
            var service = GetService();

            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportSettleInternalCSDCommand>(x => criteria = x)).Returns(tuple)
                .AndDoes(x => { criteria.ErrCode = 0; criteria.ErrMsg = "successfully"; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);

            // Act
            await service.Public_ImportSettlementInternalCSDToChatbotDBAsync(messageSettlementHeader, messageTarget, maxMsgTargetId);

            // Assert
            service.Received().Protected("OnAfterImportDataAsync", Arg.Is<CommandResult>(x => x.Code == 0 && x.Message == "successfully"), notifications);
        }
        #endregion  Settlement Internal CSD

        #region Settlement Licensee
        [Fact]
        public async Task SettlementLicenseeAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            ServiceSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(setting);

            // Act
            await service.SettlementLicenseeAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<MessageSettlement, MessageSettlementDetail, MessageSettlementInfo>(Arg.Any<GetMessageSettlementLicenseeCriteria>());
        }

        [Fact]
        public async Task SettlementLicenseeAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementDetail = fixture.Create<IEnumerable<MessageSettlementDetail>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var tuple = (messageSettlement, messageSettlementDetail, messageSettlementInfo);
            var service = GetService();

            commonService.GetServiceSetting(Arg.Any<string>()).Returns(setting);
            baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementDetail, MessageSettlementInfo>(Arg.Any<GetMessageSettlementLicenseeCriteria>()).Returns(tuple);

            // Act
            await service.SettlementLicenseeAsync();

            // Assert
            service.Received().Protected("GetMessageLicenseeAndImportToChatbotAsync", messageSettlement, messageSettlementDetail, messageSettlementInfo);
        }

        [Fact]
        public async Task GetMessageLicenseeAndImportToChatbotAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageSettlementDetail = fixture.Create<IEnumerable<MessageSettlementDetail>>();
            var messageTargets = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var messageLeagueLicensee = fixture.Create<IEnumerable<MessageLeagueLicensee>>();

            var service = GetService();
            service.Protected("GetLeagueAsync", messageSettlement).Returns(Task.FromResult(messageLeagueLicensee));

            // Act
            await service.Public_GetMessageLicenseeAndImportToChatbotAsync(messageSettlement, messageSettlementDetail, messageTargets);

            // Assert
            service.Received().Protected("ImportSettlementLicenseeToChatbotDBAsync", messageLeagueLicensee, messageSettlementDetail, messageTargets);
        }

        [Fact]
        public async Task GetLeagueAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messageSettlement = fixture.Create<IEnumerable<MessageSettlement>>();
            var messageLeagueLicensee = fixture.Create<IEnumerable<MessageLeagueLicensee>>();
            var criteria = fixture.Create<GetLeagueSettlementLicenseeCriteria>();
            var service = GetService();

            baseService.GetDataAsync<MessageLeagueLicensee>(Arg.Do<GetLeagueSettlementLicenseeCriteria>(x => criteria = x)).Returns(messageLeagueLicensee);

            // Act
            var result = await service.Public_GetLeagueAsync(messageSettlement);

            // Assert
            Assert.Equal(messageLeagueLicensee, result);
        }

        [Fact]
        public async Task ImportSettlementLicenseeToChatbotDBAsync_MessageIsNull_DoNothing()
        {
            // Arrange
            var messageSettlementHeader = new List<MessageLeagueLicensee>();
            var messageSettlementDetail = fixture.Create<IEnumerable<MessageSettlementDetail>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var service = GetService();

            // Act
            await service.Public_ImportSettlementLicenseeToChatbotDBAsync(messageSettlementHeader, messageSettlementDetail, messageSettlementInfo);

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportSettlementLicenseeCommand>());
        }

        [Fact]
        public async Task ImportSettlementLicenseeToChatbotDBAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messageSettlementHeader = fixture.Create<IEnumerable<MessageLeagueLicensee>>();
            var messageSettlementDetail = fixture.Create<IEnumerable<MessageSettlementDetail>>();
            var messageSettlementInfo = fixture.Create<IEnumerable<MessageSettlementInfo>>();
            var criteria = fixture.Create<ImportSettlementLicenseeCommand>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var tuple = (recipients, notificationMessages);
            var service = GetService();

            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportSettlementLicenseeCommand>(x => criteria = x)).Returns(tuple)
                .AndDoes(x => { criteria.ErrCode = 0; criteria.ErrMsg = "successfully"; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);   

            // Act
            await service.Public_ImportSettlementLicenseeToChatbotDBAsync(messageSettlementHeader, messageSettlementDetail, messageSettlementInfo);

            // Assert
            service.Received().Protected("OnAfterImportDataAsync", Arg.Is<CommandResult>(x => x.Code == 0 && x.Message == "successfully"), notifications);
        }
        #endregion  Settlement Licensee

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
