namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class PullMessageFromSiteMessage : BaseWorker
    {
        private readonly ISiteMessageService siteMessageService;

        public PullMessageFromSiteMessage(
            ISiteMessageService siteMessageService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogger<BaseWorker> logger,
            ILogService logService)
                : base(maintenanceService, logger, logService)
        {
            this.siteMessageService = siteMessageService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.PullMessageFromSiteMessage);
        }

        public override async Task ExecuteJobAsync()
        {
            await siteMessageService.PullMessageFromSiteMessage();
        }
    }
}
