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
    }
}