namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Notification;

    public interface ISignalRService
    {
        Task UpdateNotificationStatus(string notificationIds);

        Task InsertNewNotification(string notificationIds);

        Task NewErrNotification(IEnumerable<Notification> notifications);
    }
}