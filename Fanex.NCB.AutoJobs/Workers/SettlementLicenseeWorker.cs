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

    public class SettlementLicenseeWorker : BaseWorker
    {
        private readonly ISettlementInternalLicenseeService settlementInternalLicenseeService;

        public SettlementLicenseeWorker(
            ISettlementInternalLicenseeService settlementInternalLicenseeService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
        : base(maintenanceService, logService)
        {
            this.settlementInternalLicenseeService = settlementInternalLicenseeService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SettlementLicensee);
        }

        public override async Task ExecuteJobAsync()
        {
            await settlementInternalLicenseeService.SettlementLicenseeAsync();
        }
    }
}