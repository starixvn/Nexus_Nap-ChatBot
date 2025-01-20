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
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.ResultAmendmentInternalCSD);
        }

        public override async Task ExecuteJobAsync()
        {
            await resultAmendmentService.ResultAmendmentInternalCSDAsync();
        }
    }
}