namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Hosting;
    using ONEbook.UM.Services;

    public class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogService logService;

        private Timer timer;
        private bool isRunning;

        protected JobSetting jobSetting;

        public BaseWorker(
            IMaintenanceService maintenanceService,
            ILogService logService)
        {
            this.maintenanceService = maintenanceService;
            this.logService = logService;
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

            logService.WriteLogFileInfo($"{jobSetting.JobName}: Service Starting");
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
                    logService.WriteLogFileWarning($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logService.WriteLogFileWarning($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);
                await logService.WriteMessageSentryErrorAsync(ex.ToString());
                logService.WriteLogFileError($"{jobSetting.JobName}: Error - {ex}");
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
            logService.WriteLogFileWarning($"{jobSetting.JobName}: Stopping Service");
            timer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.WriteLogFileWarning($"{jobSetting.JobName}: Disposing Service");
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
