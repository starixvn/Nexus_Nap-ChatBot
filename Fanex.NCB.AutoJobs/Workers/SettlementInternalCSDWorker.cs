namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class SettlementInternalCSDWorker : BaseWorker
    {
        private readonly ISettlementInternalCSDService settledmentInternalCSDService;

        public SettlementInternalCSDWorker(
            ISettlementInternalCSDService settledmentInternalCSDService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.settledmentInternalCSDService = settledmentInternalCSDService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SettlementInternalCSD);
        }

        public override async Task ExecuteJobAsync()
        {
            await settledmentInternalCSDService.SettlementInternalCSDAsync();
        }
    }
}
