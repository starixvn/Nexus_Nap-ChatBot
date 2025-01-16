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
    public class ScanAndSendNotificationWorker : BaseWorker
    {
        private readonly ISendNotificationService sendNotificationService;

        public ScanAndSendNotificationWorker(
            ISendNotificationService sendNotificationService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.sendNotificationService = sendNotificationService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendNotification);
        }

        public override async Task ExecuteJobAsync()
        {
            await sendNotificationService.ScanAndSendNotificationAsync();
        }
    }
}