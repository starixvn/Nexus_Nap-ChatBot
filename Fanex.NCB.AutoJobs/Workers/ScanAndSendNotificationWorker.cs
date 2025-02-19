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
public class ScanAndSendNotificationWorker : BaseWorker
{
    private readonly ISendNotificationService sendNotificationService;

    public ScanAndSendNotificationWorker(
        ISendNotificationService sendNotificationService,
        IMaintenanceService maintenanceService,
        IOptions<List<JobSetting>> jobSettings,
        ILogService logService)
        : base(maintenanceService, logService)
    {
        this.sendNotificationService = sendNotificationService;
        JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendNotification);
    }

    public override async Task ExecuteJobAsync()
    {
        await sendNotificationService.ScanAndSendNotificationAsync();
    }
}