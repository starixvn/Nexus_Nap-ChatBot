namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.NotificationTemplate;
    using Fanex.NCB.Shared.Models;

    public interface INotificationTemplaceService
    {
        Task<(long, IEnumerable<NotificationTemplate>)> GetAsync(int? notificationTypeId, int pageIndex, int pageSize);

        Task<CommandResult> CreateAsync(CreateNotificationTemplate model);

        Task<CommandResult> EditAsync(EditNotificationTemplate model);

        Task<NotificationTemplate> GetByIdAsync(int notificationTemplateId);

        Task<CommandResult> DeleteAsync(int notificationTemplateId, int userId);
    }
}