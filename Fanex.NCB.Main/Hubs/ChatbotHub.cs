namespace Fanex.NCB.Main.Hubs
{
    using System.Threading.Tasks;
    using Services.Interfaces;
    using Microsoft.AspNetCore.SignalR;

    public class ChatbotHub : Hub<IChatbotClient>
    {
        private readonly INotificationService notificationService;

        public ChatbotHub(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task UpdateNewNotification(string notificationIds)
        {
            await Clients.All.InsertNewNotifications(notificationIds);
        }

        public async Task UpdateNotificationStatus(string notificationIds)
        {
            await Clients.All.UpdateNotificationStatus(notificationIds);
        }

        public async Task UpdateErrorNotification()
        {
            var errNotifications = await notificationService.GetErrNotificationsAsync();
            await Clients.All.UpdateErrNotification(errNotifications);
        }
    }
}