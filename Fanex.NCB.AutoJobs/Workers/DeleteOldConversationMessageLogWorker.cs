namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
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
    public class DeleteOldConversationMessageLogWorker : BaseWorker
    {
        private readonly ICommonService commonService;

        public DeleteOldConversationMessageLogWorker(
            ICommonService commonService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.commonService = commonService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.DeleteOldConversationMessageLog);
        }

        public override async Task ExecuteJobAsync()
        {
            var currrentDate = DateTimeOffset.Now.DateTime.ToString("yyyy-MM-dd hh:mm:ss");
            await commonService.DeleteOldLog(currrentDate);
        }
    }
}