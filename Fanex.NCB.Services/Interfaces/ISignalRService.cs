namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISignalRService
    {
        Task UpdateNotificationStatus(string notificationIds);
    }
}