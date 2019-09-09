namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.NotificationTemplate;

    public interface INotificationTemplaceService
    {
        Task<(long, IEnumerable<NotificationTemplate>)> GetAsync(int? notificationTypeId, int pageIndex, int pageSize);
    }
}