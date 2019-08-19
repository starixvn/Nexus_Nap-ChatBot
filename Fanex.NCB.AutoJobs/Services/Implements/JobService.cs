namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;

    public class JobService : IJobService
    {
        private readonly ISendNotificationService sendNotificationService;

        public JobService(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task AutoScanAndSendNotifictaion()
        {
            await sendNotificationService.ScanAndSendNotification().ConfigureAwait(false);
        }
    }
}