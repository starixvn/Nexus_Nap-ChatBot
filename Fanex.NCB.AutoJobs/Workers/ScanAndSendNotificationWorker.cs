namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class ScanAndSendNotificationWorker : BaseWorker
    {
        private readonly ISendNotificationService sendNotificationService;

        public ScanAndSendNotificationWorker(
            ISendNotificationService sendNotificationService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogger<BaseWorker> logger,
            ILogService logService)
                : base(maintenanceService, logger, logService)
        {
            this.sendNotificationService = sendNotificationService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendNotification);
        }

        public override async Task ExecuteJobAsync()
        {
            await sendNotificationService.ScanAndSendNotification();
        }
    }
}
