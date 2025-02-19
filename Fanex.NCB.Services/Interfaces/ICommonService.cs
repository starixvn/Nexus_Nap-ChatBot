namespace Fanex.NCB.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Common;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Models.IntegrationMethod;
using Fanex.NCB.Shared.Models.OIC;

public interface ICommonService
{
    Task<IEnumerable<NotificationType>> GetNotificationTypesAsync();

    Task<IEnumerable<Product>> GetProductsAsync();

    Task<IEnumerable<Recipient>> GetRecipientsAsync();

    Task<IEnumerable<NotificationStatus>> GetNotificationStatusesAsync();

    Task<IEnumerable<User>> GetUsersAsync();

    Task<IEnumerable<ChannelInfo>> GetConversationChannelAsync();

    Task<IEnumerable<Platform>> GetPlatformAsync(int notificationTypeId);

    Task<string> GetJsonDataFromDbACCAsync(string tableName);

    Task<IEnumerable<IntegrationMethodInfo>> GetIntegrationMethodAsync(int notificationTypeId);

    Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications);

    Task OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notificationMessages);
}