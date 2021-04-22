namespace Fanex.NCB.AutoJobs.App_Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Services;
    using Fanex.NCB.Core.Models;
    using Hangfire;
    using Hangfire.Storage;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Resources;

    [DesignerCategory("Code")]
    public class CustomWebHostService : WebHostService
    {
        private readonly ILogger logger;
        private readonly IOptionsMonitor<List<HangfireJobSetting>> jobSettings;
        private readonly IRecurringJobManager recurringJob;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            logger = host.Services.GetService<ILogger<CustomWebHostService>>();
            jobSettings = host.Services.GetService<IOptionsMonitor<List<HangfireJobSetting>>>();
            recurringJob = host.Services.GetService<IRecurringJobManager>();
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
            //recurringJob.AddOrUpdate<IJobService>(Resources.ScanAndSendNotification,
            //    job => job.ScanAndSendNotification(),
            //    jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.SendNotification))?.CronExpression,
            //    TimeZoneInfo.Local, QueuPriority.Critical);

            recurringJob.AddOrUpdate<IJobService>(Resources.Job_SyncNapUserControlData,
                job => job.SyncDataFromNapUserControl(),
                jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.SyncNapUserControlData))?.CronExpression,
                TimeZoneInfo.Local);

            recurringJob.AddOrUpdate<IJobService>(Resources.Job_SyncOICData,
                job => job.SyncDataFromOIC(),
                jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.SyncOICData))?.CronExpression,
                TimeZoneInfo.Local);

            //recurringJob.AddOrUpdate<IJobService>(Resources.PullVoidingMessageLicensee,
            //   job => job.PullVoidingMessageLicensee(),
            //   jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.PullVoidingMessageLicensee))?.CronExpression,
            //   TimeZoneInfo.Local, QueuPriority.Critical);

            //recurringJob.AddOrUpdate<IJobService>(Resources.PullVoidingMessageInternalDepartment,
            //   job => job.PullVoidingMessageInternalDepartment(),
            //   jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.PullVoidingMessageInternalDepartment))?.CronExpression,
            //   TimeZoneInfo.Local, QueuPriority.Critical);

            //recurringJob.AddOrUpdate<IJobService>(Resources.PullMessageFromSiteMessage,
            //  job => job.PullMessageFromSiteMessage(),
            //  jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.PullMessageFromSiteMessage))?.CronExpression,
            //  TimeZoneInfo.Local, QueuPriority.Critical);

            //recurringJob.AddOrUpdate<IJobService>(Resources.MonitorChatbotService,
            //    job => job.MonitorChatbot(),
            //    jobSettings.CurrentValue.FirstOrDefault(j => j.JobName.Equals(HangfireJob.MonitorSkypebot))?.CronExpression,
            //    TimeZoneInfo.Local);
        }

        private static void RemoveCurrentHangfireServer()
        {
            IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
            var serverToRemove = monitoringApi.Servers().FirstOrDefault(s => s.Name.ToLowerInvariant().Contains(Environment.MachineName.ToLowerInvariant()));
            if (serverToRemove != null)
            {
                JobStorage.Current.GetConnection().RemoveServer(serverToRemove?.Name);
            }
        }
    }
}