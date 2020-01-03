namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System.Threading.Tasks;

    public interface IPushNotifyService
    {
        Task PushNotificationUpdatedStatus(string notificationIds);
    }
}