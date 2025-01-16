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
    public class ResultAmendmentInternalCSDWorkder : BaseWorker
    {
        private readonly IResultAmendmentService resultAmendmentService;

        public ResultAmendmentInternalCSDWorkder(
            IResultAmendmentService resultAmendmentService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
        : base(maintenanceService, logService)
        {
            this.resultAmendmentService = resultAmendmentService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.ResultAmendmentInternalCSD);
        }

        public override async Task ExecuteJobAsync()
        {
            await resultAmendmentService.ResultAmendmentInternalCSDAsync();
        }
    }
}