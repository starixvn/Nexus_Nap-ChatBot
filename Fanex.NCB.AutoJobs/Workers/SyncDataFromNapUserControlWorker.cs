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

    public class SyncDataFromNapUserControlWorker : BaseWorker
    {
        private readonly ISyncDataService syncDataService;
        private readonly ILogService logService;

        public SyncDataFromNapUserControlWorker(
            ISyncDataService syncDataService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.syncDataService = syncDataService;
            this.logService = logService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncNapUserControlData);
        }

        public override async Task ExecuteJobAsync()
        {
            try
            {
                await syncDataService.NapUserControlSyncDataAsync();
            }
            catch (System.Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);
                throw;
            }
        }
    }
}