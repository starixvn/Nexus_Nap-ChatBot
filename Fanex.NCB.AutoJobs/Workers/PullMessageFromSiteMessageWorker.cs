namespace Fanex.NCB.AutoJobs.Workers;

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
public class PullMessageFromSiteMessage : BaseWorker
{
    private readonly ISiteMessageService siteMessageService;

    public PullMessageFromSiteMessage(
        ISiteMessageService siteMessageService,
        IMaintenanceService maintenanceService,
        IOptions<List<JobSetting>> jobSettings,
        ILogService logService)
        : base(maintenanceService, logService)
    {
        this.siteMessageService = siteMessageService;
        JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.PullMessageFromSiteMessage);
    }

    public override async Task ExecuteJobAsync()
    {
        await siteMessageService.PullMessageFromSiteMessageAsync();
    }
}