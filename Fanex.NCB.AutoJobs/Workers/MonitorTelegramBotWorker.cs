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
    public class MonitorTelegramBotWorker : BaseWorker
    {
        private readonly IChatbotService chatbotService;

        public MonitorTelegramBotWorker(
            IChatbotService chatbotService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.chatbotService = chatbotService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.MonitorTelegramBot);
        }

        public override async Task ExecuteJobAsync()
        {
            await chatbotService.MonitorTelegramBotAsync();
        }
    }
}