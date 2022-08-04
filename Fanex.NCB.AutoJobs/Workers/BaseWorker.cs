namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Hosting;
    using ONEbook.UM.Services;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogService logService;

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
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunJobAsync();

                    await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
                }

                logService.WriteLogFileInfo($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                logService.WriteLogFileInfo("Unhandeled exception. Execution Stopping");
                logService.WriteLogFileInfo(ex.Message);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled
                    && (!jobSetting.HasCheckUM || !IsUM()))
                {
                    logService.WriteLogFileWarning($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logService.WriteLogFileWarning($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                _ = logService.WriteExceptionSentryErrorAsync(ex);
                logService.WriteLogFileError($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        public abstract Task ExecuteJobAsync();

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileWarning($"{jobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.WriteLogFileWarning($"{jobSetting.JobName}: Disposing Service");
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private bool IsUM()
        {
            var maintanceTime = maintenanceService.GetUnderMaintenanceTime();
            return maintanceTime.IsUnderMaintenanceTime;
        }
    }
}