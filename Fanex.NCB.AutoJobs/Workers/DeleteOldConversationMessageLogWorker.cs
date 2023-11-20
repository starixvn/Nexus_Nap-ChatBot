namespace Fanex.NCB.AutoJobs.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class DeleteOldConversationMessageLogWorker : BaseWorker
    {
        private readonly IViewLogService viewLogService;

        public DeleteOldConversationMessageLogWorker(
            IViewLogService viewLogService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.viewLogService = viewLogService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.DeleteOldConversationMessageLog);
        }

        public override async Task ExecuteJobAsync()
        {
            var currrentDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            await viewLogService.DeleteOldConversationMessageLogAsync(currrentDate);
        }
    }
}