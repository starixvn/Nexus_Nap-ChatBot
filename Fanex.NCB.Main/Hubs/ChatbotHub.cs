namespace Fanex.NCB.Main.Hubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Notification;
    using Microsoft.AspNetCore.SignalR;

    public class ChatbotHub : Hub<IChatbotClient>
    {
        public async Task UpdateNewNotification(string notificationIds)
        {
            await Clients.All.InsertNewNotifications(notificationIds);
        }

        public async Task UpdateNotificationStatus(string notificationIds)
        {
            await Clients.All.UpdateNotificationStatus(notificationIds);
        }

        public async Task UpdateErrorNotification(IEnumerable<Notification> notifications)
        {
            await Clients.All.UpdateErrNotification(notifications);
        }
    }
}