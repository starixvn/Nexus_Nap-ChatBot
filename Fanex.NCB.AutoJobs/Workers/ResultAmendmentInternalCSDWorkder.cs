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