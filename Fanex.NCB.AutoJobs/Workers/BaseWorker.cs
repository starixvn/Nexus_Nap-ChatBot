namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogger<BaseWorker> logger;
        private readonly ILogService logService;

        private Timer timer;
        private bool isRunning;

        protected JobSetting jobSetting;

        public BaseWorker(
            IMaintenanceService maintenanceService,
            ILogger<BaseWorker> logger,
            ILogService logService)
        {
            this.maintenanceService = maintenanceService;
            this.logService = logService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(Execute,
               null,
               TimeSpan.Zero,
               TimeSpan.FromSeconds(jobSetting.TimeInterval));

            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{jobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private async void Execute(object state)
        {
            try
            {
                if (!isRunning
                    && jobSetting.IsEnabled
                    && (!jobSetting.HasCheckUM || !IsUM()))
                {
                    isRunning = true;
                    logger.LogInformation($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logger.LogInformation($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{jobSetting.JobName}: Error - {ex}");
                await logService.WriteExceptionSentryErrorAsync(ex);
            }
            finally
            {
                isRunning = false;
            }
        }

        public virtual async Task ExecuteJobAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{jobSetting.JobName}: Stopping Service");
            timer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logger.LogInformation($"{jobSetting.JobName}: Disposing Service");
            timer.Dispose();
            base.Dispose();
        }

        private bool IsUM()
        {
            var maintanceTime = maintenanceService.GetUnderMaintenanceTime();
            return maintanceTime.IsUnderMaintenanceTime;
        }
    }
}
