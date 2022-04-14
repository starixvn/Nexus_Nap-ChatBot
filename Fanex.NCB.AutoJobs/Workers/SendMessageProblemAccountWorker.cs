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
            var t  = await problemAccountService.GetMessageSetting();
        }
    }
}
