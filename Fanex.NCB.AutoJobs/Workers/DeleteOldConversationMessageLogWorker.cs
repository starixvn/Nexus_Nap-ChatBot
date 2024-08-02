namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

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