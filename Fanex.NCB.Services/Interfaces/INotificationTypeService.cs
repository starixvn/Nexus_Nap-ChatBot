namespace Fanex.NCB.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Models.NotificationType;
using Fanex.NCB.Shared.Models;

public interface INotificationTypeService
{
    Task<(IEnumerable<NotificationType>, long)> GetAllAsync(int pageIndex, int pageSize);

    Task<CommandResult> DeleteAsync(int notificationTypeId, int userId);

    Task<CommandResult> InsertAsync(NotificationTypeViewModel model);

    Task<CommandResult> UpdateAsync(NotificationTypeViewModel model);
}