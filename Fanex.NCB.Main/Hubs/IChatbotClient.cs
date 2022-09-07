namespace Fanex.NCB.Main.Hubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Notification;

    public interface IChatbotClient
    {
        Task InsertNewNotifications(string notificationIds);

        Task UpdateNotificationStatus(string notificationIds);

        Task UpdateErrNotification(IEnumerable<Notification> notifications);
    }
}