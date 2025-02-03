namespace Fanex.NCB.Services.Interfaces;

using System.Threading.Tasks;

public interface IPushSignalRService
{
    Task PushNewNotification(string notificationIds);

    Task PushNotificationUpdateStatus(string notificationIds);

    Task PushErrorNotification();
}