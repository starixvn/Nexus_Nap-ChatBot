namespace Fanex.NCB.AutoJobs.Workers
{
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