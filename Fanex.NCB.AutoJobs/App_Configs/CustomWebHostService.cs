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
        private readonly IOptionsMonitor<CronJob> cronJob;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
            cronJob = host.Services.GetRequiredService<IOptionsMonitor<CronJob>>();
        }

        protected override void OnStarting(string[] args)
        {
            Debugger.Launch();
            logger.LogInformation($"Nap Chatbot Staring service at {DateTime.Now}.");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            logger.LogInformation($"Nap Chatbot Service started & initialize auto jobs at {DateTime.Now}.");
            RunHangfireJobs();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            logger.LogInformation($"Stopping Nap Chatbot service & remove Hangfire schedule jobs at {DateTime.Now}");
            RemoveHangfireJobs();
            base.OnStopping();
        }

        private void RunHangfireJobs()
        {
            RecurringJob.AddOrUpdate<IJobService>("Auto Scan & Send Notification",
                job => job.AutoScanAndSendNotifictaion(),
                cronJob.CurrentValue.SendNotification);

            RecurringJob.AddOrUpdate<IJobService>("Auto sync data from Nap User Control",
                job => job.SyncDataFromNapUserControl(),
                cronJob.CurrentValue.SyncNapUserControlData);
        }

        private static void RemoveHangfireJobs()
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