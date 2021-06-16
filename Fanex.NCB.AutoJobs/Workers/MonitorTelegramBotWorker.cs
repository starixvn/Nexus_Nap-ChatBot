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