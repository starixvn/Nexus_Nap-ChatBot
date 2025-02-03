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
public class SendMessageProblemAccountWorker : BaseWorker
{
    private readonly IProblemAccountService problemAccountService;

    public SendMessageProblemAccountWorker(
        IProblemAccountService problemAccountService,
        IMaintenanceService maintenanceService,
        IOptions<List<JobSetting>> jobSettings,
        ILogService logService)
        : base(maintenanceService, logService)
    {
        this.problemAccountService = problemAccountService;
        JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendMesageProblemAccount);
    }

    public override async Task ExecuteJobAsync()
    {
        await problemAccountService.SendMessageProblemAccountAsync();
    }
}