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
    using Resources;

    [DesignerCategory("Code")]
    internal class CustomWebHostService : WebHostService
    {
        private readonly ILogger logger;
        private readonly IOptionsMonitor<CronJob> cronJob;
        private readonly IRecurringJobManager recurringJob;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
            cronJob = host.Services.GetRequiredService<IOptionsMonitor<CronJob>>();
            recurringJob = host.Services.GetRequiredService<IRecurringJobManager>();
        }

        protected override void OnStarting(string[] args)
        {
            logger.LogInformation(string.Format(Resources.Log_OnStartingService, DateTime.Now));
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            logger.LogInformation(string.Format(Resources.Log_OnStartedService, DateTime.Now));
            RunHangfireJobs();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            logger.LogInformation(string.Format(Resources.Log_OnStoppingService, DateTime.Now));
            RemoveCurrentHangfireServer();
            base.OnStopping();
        }

        public void RunHangfireJobs()
        {
            recurringJob.AddOrUpdate<IJobService>(Resources.Job_ScanAndSendNotification,
                job => job.AutoScanAndSendNotifictaion(),
                cronJob.CurrentValue.SendNotification);

            recurringJob.AddOrUpdate<IJobService>(Resources.Job_SyncNapUserControlData,
                job => job.SyncDataFromNapUserControl(),
                cronJob.CurrentValue.SyncNapUserControlData);

            recurringJob.AddOrUpdate<IJobService>(Resources.Job_SyncOICData,
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