namespace Fanex.NCB.AutoJobs.App_Configs
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Fanex.NCB.AutoJobs.Services;
    using Hangfire;
    using Hangfire.Storage;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [DesignerCategory("Code")]
    internal class CustomWebHostService : WebHostService
    {
        private readonly ILogger logger;
        private readonly IOptionsSnapshot<CronJob> cronJob;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
            cronJob = host.Services.GetRequiredService<IOptionsSnapshot<CronJob>>();
        }

        protected override void OnStarting(string[] args)
        {
            Debugger.Launch();
            logger.LogInformation($"Staring service at {DateTime.Now}.");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            logger.LogInformation($"Service started at {DateTime.Now}.");
            RemoveHangfireJobs();
            RunHangfireJobs();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            logger.LogInformation($"Stopping service at {DateTime.Now}");
            base.OnStopping();
        }

        private void RunHangfireJobs()
        {
            RecurringJob.AddOrUpdate<IJobService>("Auto Scan & Send Notification",
                sendNoti => sendNoti.AutoScanAndSendNotifictaion(),
                cronJob.Value.SendNotification);
        }

        private void RemoveHangfireJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
    }
}