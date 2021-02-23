namespace Fanex.NCB.AutoJobs.Services
{
    using Fanex.NCB.AutoJobs.Constants;
    using Hangfire;
    using System.Threading.Tasks;

    public interface IJobService
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        [DisableConcurrentExecution(0)]
        Task SyncDataFromNapUserControl();

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        [DisableConcurrentExecution(0)]
        Task SyncDataFromOIC();

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        [DisableConcurrentExecution(0)]
        Task PullMessageFromSpecialDelete();

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        [DisableConcurrentExecution(0)]
        Task PullMessageFromSiteMessage();

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Critical)]
        [DisableConcurrentExecution(0)]
        Task ScanAndSendNotification();

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [Queue(QueuPriority.Default)]
        [DisableConcurrentExecution(0)]
        Task MonitorChatbot();
    }
}