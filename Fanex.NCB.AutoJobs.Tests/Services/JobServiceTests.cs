namespace Fanex.NCB.AutoJobs.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Services.Interfaces;
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
        private ILogger logger;

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

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.SyncDataFromNapUserControl());
            await logger.Received().ErrorAsync(Arg.Any<string>());
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

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.SyncDataFromOIC());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task PullTicketFromSpecialDeleteAndCreateNotification_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.PullMessageFromSpecialDelete();

            //Assert
            await specialDeleteTicketService.Received().VoidingTicketToLicensee();
        }

        [Fact]
        public async Task PullTicketFromSpecialDeleteAndCreateNotification_WhenCalled_ThrowException()
        {
            var service = GetService();
            specialDeleteTicketService.VoidingTicketToLicensee().Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.PullMessageFromSpecialDelete());
            await logger.Received().ErrorAsync(Arg.Any<string>());
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

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.ScanAndSendNotification());
            await logger.Received().ErrorAsync(Arg.Any<string>());
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

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.MonitorChatbot());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        #region Helper

        private JobService GetService()
        {
            syncDataService = Substitute.For<ISyncDataService>();
            chatbotService = Substitute.For<IChatbotService>();
            specialDeleteTicketService = Substitute.For<ISpecialDeleteService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();
            maintenanceService = Substitute.For<IMaintenanceService>();
            logger = Substitute.For<ILogger>();
            var um = new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = false
            };
            maintenanceService.GetUnderMaintenanceTimeAsync().Returns(um);

            return new JobService(syncDataService, chatbotService, specialDeleteTicketService, sendNotificationService, maintenanceService,
               logger);
        }

        #endregion Helper
    }
}