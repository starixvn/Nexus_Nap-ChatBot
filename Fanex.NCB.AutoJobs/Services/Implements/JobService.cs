namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;

    public class JobService : IJobService
    {
        private readonly ISendNotificationService sendNotificationService;
        private readonly ISyncDataService syncDataService;

        public JobService(ISendNotificationService sendNotificationService, ISyncDataService syncDataService)
        {
            this.sendNotificationService = sendNotificationService;
            this.syncDataService = syncDataService;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task AutoScanAndSendNotifictaion()
        {
            await sendNotificationService.ScanAndSendNotification().ConfigureAwait(false);
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromNapUserControl()
        {
            await syncDataService.NapUserControlSyncDataAsync().ConfigureAwait(false);
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SyncDataFromOIC()
        {
            await syncDataService.OicSyncDataAsync().ConfigureAwait(false);
        }
    }
}