namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Shared.Models;

    public interface INotificationService
    {
        Task<CommandResult> CreateAsync(CreateNotification model);

        Task<IEnumerable<Recipient>> GetRecipientsAsync(int notificationTypeId, int? productId);

        Task<(long, IEnumerable<Notification>)> GetNotificationsAsync(GetNotifications model);

        Task<CommandResult> CancelAsync(int notificationId, int userId);

        Task<NotificationErrorReport> GetNotificationErrorAsync(int notificationId);

        Task<CommandResult> ResolveAsync(int notificationId, string resolvedRemark, int userId);

        Task<EditNotificationInfo> GetEditNotificationInfoAsync(int notificationId);

        Task<CommandResult> EditAsync(EditNotification model);

        Task<IEnumerable<Recipient>> GetEditNotificationRecipientsAsync(int notificationId, int notificationTypeId, int? productId);

        Task<IEnumerable<Template>> GetTemplatesAsync(int notificationTypeId);

        Task<ResendNotification> GetResendNotificationInfoAsync(int notificationId, bool isResendAll);
        
        Task<IEnumerable<Notification>> GetUpdatedNotificationAsync(string notificationIds);

        Task<IEnumerable<Notification>> GetAllErrorNotificationsAsync(GetErrorNotifications model);

        Task<IEnumerable<Notification>> GetErrNotificationsAsync();
    }
}