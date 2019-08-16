namespace SchedulerJobs.Services
{
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task AutoScanAndSendNotifictaion();
    }
}