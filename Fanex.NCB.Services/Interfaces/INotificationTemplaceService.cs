namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Shared.Models;

    public interface INotificationTemplaceService
    {
        Task<(long, IEnumerable<NotificationTemplate>)> GetAsync(int? notificationTypeId, int pageIndex, int pageSize);

        Task<CommandResult> CreateAsync(CreateNotificationTemplate model);
    }
}