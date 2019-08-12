namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISendNotificationService
    {
        Task ScanAndSendNotification(int creatorId);

        Task SendNotificationImediately(int notifiactionId, int creatorId);
    }
}