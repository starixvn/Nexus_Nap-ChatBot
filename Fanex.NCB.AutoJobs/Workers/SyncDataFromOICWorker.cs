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
    public class SyncDataFromOICWorker : BaseWorker
    {
        private readonly ISyncDataService syncDataService;

        public SyncDataFromOICWorker(
            ISyncDataService syncDataService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.syncDataService = syncDataService;
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncOICData);
        }

        public override async Task ExecuteJobAsync()
        {
            await syncDataService.OicSyncDataAsync();
        }
    }
}