namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    [ExcludeFromCodeCoverage(Justification = "Worker don't needed test")]
    public class ResultAmendmentLicenseeWorkder : BaseWorker
    {
        private readonly IResultAmendmentService resultAmendmentService;

        public ResultAmendmentLicenseeWorkder(
            IResultAmendmentService resultAmendmentService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
        : base(maintenanceService, logService)
        {
            this.resultAmendmentService = resultAmendmentService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.ResultAmendmentILicensee);
        }

        public override async Task ExecuteJobAsync()
        {
            await resultAmendmentService.ResultAmendmentLicenseeAsync();
        }
    }
}