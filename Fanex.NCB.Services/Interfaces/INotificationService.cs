namespace Fanex.NCB.Services.Interfaces
{
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Shared.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INotificationService
    {
        Task<CommandResult> CreateAsync(CreateNotification model);

        Task<IEnumerable<Recipient>> GetRecipientsAsync(int notificationTypeId, int? productId);

        Task<(long, IEnumerable<Notification>)> GetNotificationsAsync(GetNotifications model);

        Task<CommandResult> CancelAsync(int notificationId, int userId);

        Task<NotificationErrorReport> GetNotificationErrorAsync(int notificationId);

        Task<CommandResult> ResolveAsync(int notificationId, string resolvedRemark, int userId);

        Task<EditNotificationInfo> GetEditNotificationInfo(int notificationId);

        Task<CommandResult> EditAsync(EditNotification model);
    }
}