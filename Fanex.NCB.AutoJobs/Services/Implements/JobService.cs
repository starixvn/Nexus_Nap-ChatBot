namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;

    public class JobService : IJobService
    {
        private readonly ISendNotificationService sendNotificationService;
        private readonly ISyncDataService syncDataService;
        private readonly ILogger logger;

        public JobService(ISendNotificationService sendNotificationService,
            ISyncDataService syncDataService,
            ILogger logger)
        {
            this.sendNotificationService = sendNotificationService;
            this.syncDataService = syncDataService;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task AutoScanAndSendNotifictaion()
        {
            try
            {
                await sendNotificationService.ScanAndSendNotification();
            }
            catch (Exception ex)
            {
                Logging(ex);
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromNapUserControl()
        {
            try
            {
                await syncDataService.NapUserControlSyncDataAsync();
            }
            catch (Exception ex)
            {
                Logging(ex);
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromOIC()
        {
            try
            {
                await syncDataService.OicSyncDataAsync();
            }
            catch (Exception ex)
            {
                Logging(ex);
            }
        }

        private void Logging(Exception exception)
        {
            string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    string.Empty,
                    exception.TargetSite,
                    exception.StackTrace);
            logger.Error(message);
        }
    }
}