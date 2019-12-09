namespace Fanex.NCB.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task AutoScanAndSendNotifictaion();

        Task SyncDataFromNapUserControl();

        Task SyncDataFromOIC();

        Task PullTicketFromSpecialDeleteAndCreateNotification();

        Task MonitorChatbot();
    }
}