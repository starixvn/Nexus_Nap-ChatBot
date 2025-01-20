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
    public class PullVoidingMessageLicenseeWorker : BaseWorker
    {
        private readonly ISpecialDeleteService specialDeleteTicketService;

        public PullVoidingMessageLicenseeWorker(
            ISpecialDeleteService specialDeleteTicketService,
            IMaintenanceService maintenanceService,
            IOptions<List<JobSetting>> jobSettings,
            ILogService logService)
                : base(maintenanceService, logService)
        {
            this.specialDeleteTicketService = specialDeleteTicketService;
            JobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.PullVoidingMessageLicensee);
        }

        public override async Task ExecuteJobAsync()
        {
            await specialDeleteTicketService.PullVoidingMessageLicenseeAsync();
        }
    }
}