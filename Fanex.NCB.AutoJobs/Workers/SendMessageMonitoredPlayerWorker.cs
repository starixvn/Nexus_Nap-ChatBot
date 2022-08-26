namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

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
            await monitoredPlayerService.SendMessageMontitoredPlayerAsync();
        }
    }
}
