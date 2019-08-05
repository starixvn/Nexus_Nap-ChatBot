namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.NotificationType;
    using Fanex.NCB.Shared.Models;

    public interface INotificationTypeService
    {
        Task<(IEnumerable<NotificationType>, long)> GetAllAsync(int pageIndex, int pageSize);

        Task<CommandResult> DeleteAsync(int notificationTypeId);

        Task<CommandResult> InsertAsync(NotificationType model);

        Task<CommandResult> UpdateAsync(NotificationType model);

        Task<IEnumerable<NotificationRecipientType>> GetRecipientType();

        Task<IEnumerable<BaseModel>> GetSystem();
    }
}