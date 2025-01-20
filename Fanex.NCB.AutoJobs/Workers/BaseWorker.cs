namespace Fanex.NCB.AutoJobs.Workers
{
    using Core.Services.Interfaces;
    using Models;
    using ONEbook.UM.Services;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage(Justification = "Code Starup inject needed services")]
    public abstract class BaseWorker : BackgroundService
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly ILogService logService;

        protected JobSetting JobSetting;

        protected BaseWorker(
            IMaintenanceService maintenanceService,
            ILogService logService)
        {
            this.maintenanceService = maintenanceService;
            this.logService = logService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte millisecondsDelay = 10;
            await Task.Delay(millisecondsDelay, stoppingToken).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunJobAsync();

                    await Task.Delay(TimeSpan.FromSeconds(JobSetting.TimeInterval), stoppingToken);
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
            logService.WriteLogFileInfo($"{JobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (JobSetting.IsEnabled
                    && (!JobSetting.HasCheckUM || !IsUM()))
                {
                    logService.WriteLogFileWarning($"{JobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logService.WriteLogFileWarning($"{JobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                _ = logService.WriteExceptionSentryErrorAsync(ex);
                logService.WriteLogFileError($"{JobSetting.JobName}: Error - {ex}");
            }
        }

        public abstract Task ExecuteJobAsync();

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileWarning($"{JobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.WriteLogFileWarning($"{JobSetting.JobName}: Disposing Service");
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private bool IsUM()
        {
            var maintanceTime = maintenanceService.GetUnderMaintenanceTime();
            logService.WriteLogFileWarning($"Job: {JobSetting.JobName} - Is UM: {maintanceTime.IsUnderMaintenanceTime}");
            return maintanceTime.IsUnderMaintenanceTime;
        }
    }
}