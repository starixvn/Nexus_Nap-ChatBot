namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Constants;
    using Core.Services.Interfaces;
    using Extensions;
    using Models;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

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
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendMesageMonitoredPlayer);
        }

        public override async Task ExecuteJobAsync()
        {
            await monitoredPlayerService.SendMessageMonitoredPlayerAsync();
        }
    }
}