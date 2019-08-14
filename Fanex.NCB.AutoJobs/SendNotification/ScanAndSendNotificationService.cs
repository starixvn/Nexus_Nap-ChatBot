using Fanex.NCB.Services.Interfaces;

namespace Fanex.NCB.AutoJobs.SendNotification
{
    public class ScanAndSendNotificationService
    {
        private readonly ISendNotificationService sendNotificationService;

        public ScanAndSendNotificationService(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }
    }
}