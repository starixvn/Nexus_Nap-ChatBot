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
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.MonitorTelegramBot);
        }

        public override async Task ExecuteJobAsync()
        {
            await chatbotService.MonitorTelegramBotAsync();
        }
    }
}