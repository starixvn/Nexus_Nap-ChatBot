namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

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
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SendMesageProblemAccount);
        }

        public override async Task ExecuteJobAsync()
        {
            await problemAccountService.SendMessageProblemAccountAsync();
        }
    }
}