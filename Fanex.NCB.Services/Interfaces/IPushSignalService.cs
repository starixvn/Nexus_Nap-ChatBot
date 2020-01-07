namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IPushSignalService
    {
        Task OnUpdateNotificationStatus(string notificationIds);

        Task OnInsertNotification(string notificationIds);
    }
}