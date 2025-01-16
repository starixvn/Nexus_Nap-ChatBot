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