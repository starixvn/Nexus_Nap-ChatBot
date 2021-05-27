namespace Fanex.NCB.Services.Interfaces
{
    using Fanex.NCB.Models.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommonService
    {
        Task<IEnumerable<NotificationType>> GetNotificationTypesAsync();

        Task<IEnumerable<Product>> GetProductsAsync();

        Task<IEnumerable<Recipient>> GetRecipientsAsync();

        Task<IEnumerable<NotificationStatus>> GetNotificationStatusesAsync();

        Task<IEnumerable<User>> GetUsersAsync();

        Task<IEnumerable<ConversationChannel>> GetConversationChannelAsync();
    }
}