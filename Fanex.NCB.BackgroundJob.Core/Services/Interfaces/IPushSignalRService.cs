namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces;

using System.Threading.Tasks;

public interface IPushSignalRService
{
    Task PushNewNotification(string notificationIds);

    Task PushNotificationUpdateStatus(string notificationIds);

    Task PushErrorNotification();
}