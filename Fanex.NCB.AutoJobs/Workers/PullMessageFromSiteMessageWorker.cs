namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class PullMessageFromSiteMessage : BaseWorker
    {
        private readonly ISiteMessageService siteMessageService;

        public PullMessageFromSiteMessage(
            ISiteMessageService siteMessageService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.siteMessageService = siteMessageService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.PullMessageFromSiteMessage);
        }

        public override async Task ExecuteJobAsync()
        {
            await siteMessageService.PullMessageFromSiteMessageAsync();
        }
    }
}