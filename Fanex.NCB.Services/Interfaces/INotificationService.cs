namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Shared.Models;

    public interface INotificationService
    {
        Task<(CommandResult, string)> CreateAsync(CreateNotification model);

        Task<(IEnumerable<Recipient>, IEnumerable<uint>)> GetRecipientsAsync(
            int notificationTypeId, string platforms, string recipientIds, int? productId = null,
            string integrationMethodIds = null, string channelIds = null
        );

        Task<(long, IEnumerable<Notification>)> GetNotificationsAsync(GetNotifications model);

        Task<CommandResult> CancelAsync(int notificationId, int userId);

        Task<NotificationErrorReport> GetNotificationErrorAsync(int notificationId);

        Task<CommandResult> ResolveAsync(int notificationId, string resolvedRemark, int userId);

        Task<EditNotificationInfo> GetEditNotificationInfoAsync(int notificationId);

        Task<CommandResult> EditAsync(EditNotification model);

        Task<(IEnumerable<Recipient>, IEnumerable<uint>)> GetEditNotificationRecipientsAsync(
            int notificationId, int notificationTypeId, int? productId, string platforms,
            string recipientIds, string integrationMethodIds
        );

        Task<IEnumerable<Template>> GetTemplatesAsync(int notificationTypeId);

        Task<ResendNotification> GetResendNotificationInfoAsync(int notificationId, bool isResendAll);

        Task<IEnumerable<Notification>> GetUpdatedNotificationAsync(string notificationIds);

        Task<IEnumerable<NotificationPaging>> GetErrNotificationsAsync();
    }
}