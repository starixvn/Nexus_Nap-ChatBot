namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.Notification;

    public interface INotificationRepository
    {
        Task<EditNotificationRepository> FetchEditNotificationInfo(ICriteria criteria);

        Task<ResendNotificationRepository> FetchResendNotificationInfo(ICriteria criteria);
    }
}
