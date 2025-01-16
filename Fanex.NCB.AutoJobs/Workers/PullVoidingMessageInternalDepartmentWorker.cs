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
            await specialDeleteTicketService.PullVoidingMessageInternalDepartmentAsync();
        }
    }
}