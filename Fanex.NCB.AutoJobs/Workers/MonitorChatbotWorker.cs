namespace Fanex.NCB.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Extensions;
    using Fanex.NCB.AutoJobs.Models;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ONEbook.UM.Services;

    public class MonitorChatbotWorker : BaseWorker
    {
        private readonly IChatbotService chatbotService;

        public MonitorChatbotWorker(
            IChatbotService chatbotService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogger<BaseWorker> logger,
            ILogService logService) 
                : base(maintenanceService, logger, logService)
        {
            this.chatbotService = chatbotService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.MonitorSkypebot);
        }

        public override async Task ExecuteJobAsync()
        {
            await chatbotService.MonitorChatbot();
        }
    }
}
