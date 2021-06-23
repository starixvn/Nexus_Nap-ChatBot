namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IPushSignalService
    {
        Task OnUpdateNotificationStatusAsync(string notificationIds);

        Task OnInsertNotificationAsync(string notificationIds);

        Task OnHasNewErrNotificationAsync();
    }
}