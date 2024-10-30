namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.ViewLog;

    public interface IViewLogService
    {
        Task<(int, IEnumerable<ViewLogNotificationSystemMessage>)> GetNotificationSystemMessageAsync(int? notificationId, int pageIndex, int pageSize);

        Task<(int, IEnumerable<ViewLogNotificationRecipientList>)> GetNotificationRecipientListAsync(int? notificationId, int pageIndex, int pageSize);

        Task<(int, IEnumerable<ViewLogConversationMessageLog>, IEnumerable<ViewLogConversationMessageLogRecipient>)> GetConversationMessageLogAsync(int? notificationId, int pageIndex, int pageSize);
    }
}