namespace Fanex.NCB.AutoJobs.App_Configs
{
    using System;
    using System.ComponentModel;
    using System.Linq;
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
            RemoveCurrentHangfireServer();
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

            RecurringJob.AddOrUpdate<IJobService>("Auto sync data from OIC",
                job => job.SyncDataFromOIC(),
                cronJob.CurrentValue.SyncOICData);
        }

        private static void RemoveCurrentHangfireServer()
        {
            IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
            var serverToRemove = monitoringApi.Servers().FirstOrDefault(s => s.Name.Equals(Environment.MachineName));
            JobStorage.Current.GetConnection().RemoveServer(serverToRemove.Name);
        }
    }
}