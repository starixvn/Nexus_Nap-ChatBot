namespace Fanex.NCB.AutoJobs.Tests.Services
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class JobServiceTests
    {
        private ISendNotificationService sendNotificationService;
        private ISyncDataService syncDataService;
        private ILogger logger;

        [Fact]
        public async Task AutoScanAndSendNotifictaion_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.AutoScanAndSendNotifictaion();

            //Assert
            await sendNotificationService.Received().ScanAndSendNotification();
        }

        #region Helper

        private JobService GetService()
        {
            sendNotificationService = Substitute.For<ISendNotificationService>();
            syncDataService = Substitute.For<ISyncDataService>();

            logger = Substitute.For<ILogger>();
            return new JobService(sendNotificationService, syncDataService, logger);
        }

        #endregion Helper
    }
}