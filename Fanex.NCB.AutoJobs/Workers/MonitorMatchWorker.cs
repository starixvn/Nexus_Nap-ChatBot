namespace Fanex.NCB.AutoJobs.Workers;

using Fanex.NCB.AutoJobs.Constants;
using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
using Fanex.NCB.AutoJobs.Extensions;
using Fanex.NCB.AutoJobs.Models;
using Microsoft.Extensions.Options;
using ONEbook.UM.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

[ExcludeFromCodeCoverage(Justification = "Worker don't needed test")]
public class MonitorMatchWorker : BaseWorker
{
    private readonly IMonitorMatchService monitorMatchService;

    public MonitorMatchWorker(
        IMaintenanceService maintenanceService, 
        ILogService logService,
        IOptions<List<JobSetting>> jobSettings,
        IMonitorMatchService monitorMatchService
        ) : base(maintenanceService, logService)
    {

        JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.MonitorMatch);
        this.monitorMatchService = monitorMatchService;
    }

    public override async Task ExecuteJobAsync()
    {
        await monitorMatchService.SendMessageMonitoredMatchAsync();
    }
}
