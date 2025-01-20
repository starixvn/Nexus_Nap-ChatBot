namespace Fanex.NCB.AutoJobs.Workers
{
    using Constants;
    using Core.Services.Interfaces;
    using Extensions;
    using Microsoft.Extensions.Options;
    using Models;
    using ONEbook.UM.Services;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

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
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.DeleteOldConversationMessageLog);
        }

        public override async Task ExecuteJobAsync()
        {
            var currrentDate = DateTimeOffset.Now.DateTime.ToString("yyyy-MM-dd hh:mm:ss");
            await commonService.DeleteOldLog(currrentDate);
        }
    }
}