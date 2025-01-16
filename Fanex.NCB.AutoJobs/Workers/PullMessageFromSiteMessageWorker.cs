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