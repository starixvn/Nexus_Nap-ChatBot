namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.SignalR;

    public class SignalRService : Hub, ISignalRService
    {
        private readonly IHubContext<SignalRService> hubContext;

        public SignalRService(IHubContext<SignalRService> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task InsertNewNotification(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                await hubContext.Clients.All.SendAsync("InsertNewNotifications", notificationIds);
            }
        }

        public async Task UpdateNotificationStatus(string notificationIds)
        {
            if (!string.IsNullOrWhiteSpace(notificationIds))
            {
                await hubContext.Clients.All.SendAsync("UpdateNotificationStatus", notificationIds);
            }
        }

        public async Task NewErrNotification(IEnumerable<Notification> notifications)
        {
            if (notifications.Any())
            {
                await hubContext.Clients.All.SendAsync("NewErrNotification", notifications);
            }
        }
    }
}