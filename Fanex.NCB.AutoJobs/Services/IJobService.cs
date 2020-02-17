namespace Fanex.NCB.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task SyncDataFromNapUserControl();

        Task SyncDataFromOIC();

        Task PullMessageFromSpecialDelete();

        Task ScanAndSendNotification();

        Task MonitorChatbot();
    }
}