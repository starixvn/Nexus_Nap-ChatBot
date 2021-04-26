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

    public class PullVoidingMessageInternalDepartmentWorker : BaseWorker
    {
        private readonly ISpecialDeleteService specialDeleteTicketService;

        public PullVoidingMessageInternalDepartmentWorker(
            ISpecialDeleteService specialDeleteTicketService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.specialDeleteTicketService = specialDeleteTicketService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.PullVoidingMessageInternalDepartment);
        }

        public override async Task ExecuteJobAsync()
        {
            await specialDeleteTicketService.PullVoidingMessageInternalDepartment();
        }
    }
}
