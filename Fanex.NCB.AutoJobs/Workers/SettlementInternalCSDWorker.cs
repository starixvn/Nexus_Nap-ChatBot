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
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SettlementInternalCSD);
        }

        public override async Task ExecuteJobAsync()
        {
            await settlementInternalLicenseeService.SettlementInternalCSDAsync();
        }
    }
}