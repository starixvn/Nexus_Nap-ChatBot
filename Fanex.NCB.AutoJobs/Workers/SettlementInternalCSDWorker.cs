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

    public class SettlementInternalCSDWorker : BaseWorker
    {
        private readonly ISettlementInternalLicenseeService settlementInternalLicenseeService;

        public SettlementInternalCSDWorker(
            ISettlementInternalLicenseeService settlementInternalLicenseeService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.settlementInternalLicenseeService = settlementInternalLicenseeService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SettlementInternalCSD);
        }

        public override async Task ExecuteJobAsync()
        {
            await settlementInternalLicenseeService.SettlementInternalCSDAsync();
        }
    }
}