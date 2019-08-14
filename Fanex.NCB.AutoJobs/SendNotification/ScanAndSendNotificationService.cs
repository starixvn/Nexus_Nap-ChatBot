namespace Fanex.NCB.AutoJobs.SendNotification
{
    using Fanex.NCB.Services.Interfaces;

    public class ScanAndSendNotificationService
    {
        private readonly ISendNotificationService sendNotificationService;

        public ScanAndSendNotificationService(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }
    }
}