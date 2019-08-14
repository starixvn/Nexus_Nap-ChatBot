namespace Fanex.NCB.AutoJobs.SendNotification
{
    using System;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Extensions.Options;

    public class ScanAndSendNotificationService
    {
        private readonly ISendNotificationService sendNotificationService;
        private readonly IRecurringJobManager recurringJob;
        private readonly IOptionsSnapshot<HangfireCronJob> cronOptions;

        public ScanAndSendNotificationService(ISendNotificationService sendNotificationService,
            IRecurringJobManager recurringJob,
            IOptionsSnapshot<HangfireCronJob> cronOptions)
        {
            this.sendNotificationService = sendNotificationService;
            this.recurringJob = recurringJob;
            this.cronOptions = cronOptions;
        }

        [AutomaticRetry(Attempts = 0)]
        public void Initialize()
        {
            recurringJob.AddOrUpdate("Auto Scan Send Notification",
                Job.FromExpression(() => sendNotificationService.ScanAndSendNotification()),
                cronOptions.Value.ScanNotifcationToSend,
                TimeZoneInfo.Local);
        }
    }
}