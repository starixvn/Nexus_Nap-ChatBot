namespace Fanex.NCB.AutoJobs.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Xunit;

    public class JobServiceTests
    {
        private ISyncDataService syncDataService;
        private IChatbotService chatbotService;
        private ISpecialDeleteService specialDeleteTicketService;
        private ISendNotificationService sendNotificationService;
        private IMaintenanceService maintenanceService;
        private ISiteMessageService siteMessageService;
        private IOptionsSnapshot<List<HangfireJobSetting>> jobs;
        private ILogService logger;

        [Fact]
        public async Task SyncDataFromNapUserControl_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.SyncDataFromNapUserControl();

            //Assert
            await syncDataService.Received().NapUserControlSyncDataAsync();
        }

        [Fact]
        public async Task SyncDataFromNapUserControl_WhenCalled_ThrowException()
        {
            var service = GetService();
            syncDataService.NapUserControlSyncDataAsync().Throws(new Exception());

            await service.SyncDataFromNapUserControl();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task SyncDataFromOIC_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.SyncDataFromOIC();

            //Assert
            await syncDataService.Received().OicSyncDataAsync();
        }

        [Fact]
        public async Task SyncDataFromOIC_WhenCalled_ThrowException()
        {
            var service = GetService();
            syncDataService.OicSyncDataAsync().Throws(new Exception());

            await service.SyncDataFromOIC();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task PullVoidingMessageLicensee_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.PullVoidingMessageLicensee();

            //Assert
            await specialDeleteTicketService.Received().PullVoidingMessageLicensee();
        }

        [Fact]
        public async Task PullVoidingMessageLicensee_WhenCalled_ThrowException()
        {
            var service = GetService();
            specialDeleteTicketService.PullVoidingMessageLicensee().Throws(new Exception());

            await service.PullVoidingMessageLicensee();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task PullVoidingMessageInternalDepartment_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.PullVoidingMessageInternalDepartment();

            //Assert
            await specialDeleteTicketService.Received().PullVoidingMessageInternalDepartment();
        }

        [Fact]
        public async Task PullVoidingMessageInternalDepartment_WhenCalled_ThrowException()
        {
            var service = GetService();
            specialDeleteTicketService.PullVoidingMessageInternalDepartment().Throws(new Exception());

            await service.PullVoidingMessageInternalDepartment();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task PullMessageFromSiteMessage_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.PullMessageFromSiteMessage();

            //Assert
            await siteMessageService.Received().PullMessageFromSiteMessage();
        }

        [Fact]
        public async Task PullMessageFromSiteMessage_WhenCalled_ThrowException()
        {
            var service = GetService();
            siteMessageService.PullMessageFromSiteMessage().Throws(new Exception());

            await service.PullMessageFromSiteMessage();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task ScanAndScanNotification_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await sendNotificationService.Received().ScanAndSendNotification();
        }

        [Fact]
        public async Task ScanAndScanNotification_WhenCalled_ThrowException()
        {
            var service = GetService();
            sendNotificationService.ScanAndSendNotification().Throws(new Exception());

            await service.ScanAndSendNotification();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        [Fact]
        public async Task MonitorChatbot_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.MonitorChatbot();

            //Assert
            await chatbotService.Received().MonitorChatbot();
        }

        [Fact]
        public async Task MonitorChatbot_WhenCalled_ThrowException()
        {
            var service = GetService();
            chatbotService.MonitorChatbot().Throws(new Exception());

            await service.MonitorChatbot();
            //Assert
            await logger.Received().WriteExceptionSentryErrorAsync(Arg.Any<Exception>());
        }

        #region Helper

        private JobService GetService()
        {
            syncDataService = Substitute.For<ISyncDataService>();
            chatbotService = Substitute.For<IChatbotService>();
            specialDeleteTicketService = Substitute.For<ISpecialDeleteService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();
            maintenanceService = Substitute.For<IMaintenanceService>();
            siteMessageService = Substitute.For<ISiteMessageService>();
            logger = Substitute.For<ILogService>();
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();
            var hangfireJobs = new List<HangfireJobSetting>
            {
                new HangfireJobSetting {JobName = HangfireJob.SyncOICData, IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.SyncNapUserControlData, IsEnabled = true},
            };
            jobs.Value.Returns(hangfireJobs);

            var um = new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = false
            };
            maintenanceService.GetUnderMaintenanceTimeAsync().Returns(um);

            return new JobService(syncDataService, chatbotService, specialDeleteTicketService,
                sendNotificationService, maintenanceService, siteMessageService, jobs, logger);
        }

        #endregion Helper
    }
}