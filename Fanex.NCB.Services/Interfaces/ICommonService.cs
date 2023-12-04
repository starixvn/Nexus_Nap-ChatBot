namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;

    public interface ICommonService
    {
        Task<IEnumerable<NotificationType>> GetNotificationTypesAsync();

        Task<IEnumerable<Models.Common.Product>> GetProductsAsync();

        Task<IEnumerable<Recipient>> GetRecipientsAsync();

        Task<IEnumerable<NotificationStatus>> GetNotificationStatusesAsync();

        Task<IEnumerable<User>> GetUsersAsync();

        Task<IEnumerable<ConversationChannel>> GetConversationChannelAsync();

        Task<IEnumerable<Platform>> GetPlatformAsync(int notificationTypeId);

        string GetCategogyName(string categoryId, string languageCode);

        string GetCustomerClassName(string customerClass, string languageCode);

        Task<string> GetJsonDataFromDbACCAsync(string tableName);
        
        Task<ServiceSetting> GetServiceSetting(string inServiceName);

        Task<IEnumerable<IntegrationMethodInfo>> GetIntegrationMethodAsync(int notificationTypeId);
    }
}