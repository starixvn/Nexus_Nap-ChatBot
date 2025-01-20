namespace Fanex.NCB.AutoJobs.Workers
{
    using Constants;
    using Core.Services.Interfaces;
    using Extensions;
    using Microsoft.Extensions.Options;
    using Models;
    using ONEbook.UM.Services;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage(Justification = "Worker don't needed test")]
    public class SendMessageMonitoredPlayerWorker : BaseWorker
    {
        private readonly IMonitoredPlayerService monitoredPlayerService;

        public SendMessageMonitoredPlayerWorker(
            IMonitoredPlayerService monitoredPlayerService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.monitoredPlayerService = monitoredPlayerService;
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendMesageMonitoredPlayer);
        }

        public override async Task ExecuteJobAsync()
        {
            await monitoredPlayerService.SendMessageMonitoredPlayerAsync();
        }
    }
}