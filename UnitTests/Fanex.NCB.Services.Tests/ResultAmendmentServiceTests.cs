namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.ResultAmendmentInternalCSD;
    using Fanex.NCB.Criteria.ResultAmendmentLicensee;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.ResultAmendment;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class ResultAmendmentServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private ICommonService commonService;
        private IChatbotService chatbotService;

        #region ResultAmendment Internal CSD
        [Fact]
        public async Task ResultAmendmentInternalCSDAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var messageAmendmentCsd = fixture.Create<IEnumerable<MessageAmendmentCsd>>();
            var service = GetService();

            commonService.GetServiceSetting(Arg.Any<string>()).Returns(setting);
            baseService.GetDataAsync<MessageAmendmentCsd>(Arg.Any<GetMessageResultAmendmentCsdCriteria>()).Returns(messageAmendmentCsd);

            // Act
            await service.ResultAmendmentInternalCSDAsync();

            // Assert
            service.Received().Protected("ImportResultAmendmentInternalCSDAsync", messageAmendmentCsd, Arg.Any<DateTime>());
        }

        [Fact]
        public async Task ImportResultAmendmentInternalCSDAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<MessageAmendmentCsd>>();
            var criteria = fixture.Create<ImportResultAmendmentCSDCommand>();
            var newLastModifiedTime = fixture.Create<DateTime>();
            var notificationRecipient = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationSendMessage = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var tuple = (notificationRecipient, notificationSendMessage);
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();

            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportResultAmendmentCSDCommand>(x => criteria = x)).Returns(tuple)
                .AndDoes(x => { criteria.ErrCode = 0; criteria.ErrMsg = "successfully"; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationSendMessage, notificationRecipient).Returns(notifications);

            // Act
            await service.Public_ImportResultAmendmentInternalCSDAsync(messages, newLastModifiedTime);

            // Assert
            await commonService.Received(1).OnAfterImportDataAsync(Arg.Is<CommandResult>(x => x.Code == 0 && x.Message == "successfully"), notifications);
        }
        #endregion ResultAmendment Internal CSD


        #region ResultAmendment Licensee
        [Fact]
        public async Task ResultAmendmentLicenseeAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<ServiceSetting>();
            var messageAmendmentHeader = fixture.Create<IEnumerable<MessageAmendmentHeader>>();
            var messageAmendmentDetail = fixture.Create<IEnumerable<MessageAmendmentDetail>>();
            var messageAmendmentTarget = fixture.Create<IEnumerable<MessageAmendmentTarget>>();
            var tupe = (messageAmendmentHeader, messageAmendmentDetail, messageAmendmentTarget);
            var service = GetService();

            commonService.GetServiceSetting(Arg.Any<string>()).Returns(setting);
            baseService.FetchMultipleAsync<MessageAmendmentHeader, MessageAmendmentDetail, MessageAmendmentTarget>(Arg.Any<GetMessageResultAmendmentLicenseeCriteria>())
                .Returns(tupe);

            // Act
            await service.ResultAmendmentLicenseeAsync();

            // Assert
            service.Received().Protected("ImportResultAmendmentInternalLicenseeAsync", messageAmendmentHeader, messageAmendmentDetail, messageAmendmentTarget);
        }

        [Fact]
        public async Task ImportResultAmendmentInternalLicenseeAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<MessageAmendmentCsd>>();
            var criteria = fixture.Create<ImportResultAmendmentLicenseeCommand>();
            var messageAmendmentHeader = fixture.Create<IEnumerable<MessageAmendmentHeader>>();
            var messageAmendmentDetail = fixture.Create<IEnumerable<MessageAmendmentDetail>>();
            var messageAmendmentTarget = fixture.Create<IEnumerable<MessageAmendmentTarget>>();
            var notificationRecipient = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationSendMessage = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var tuple = (notificationRecipient, notificationSendMessage);
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();

            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportResultAmendmentLicenseeCommand>(x => criteria = x)).Returns(tuple)
                .AndDoes(x => { criteria.ErrCode = 0; criteria.ErrMsg = "successfully"; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationSendMessage, notificationRecipient).Returns(notifications);

            // Act
            await service.Public_ImportResultAmendmentInternalLicenseeAsync(messageAmendmentHeader, messageAmendmentDetail, messageAmendmentTarget);

            // Assert
            await commonService.Received(1).OnAfterImportDataAsync(Arg.Is<CommandResult>(x => x.Code == 0 && x.Message == "successfully"), notifications);
        }
        #endregion ResultAmendment Licensee


        #region Helpers
        private ResultAmendmentServiceTestable GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            chatbotService = Substitute.For<IChatbotService>();

            return Substitute.For<ResultAmendmentServiceTestable>(baseService, chatbotService, commonService);
        }
        #endregion Helpers
    }
}
