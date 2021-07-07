namespace Fanex.NCB.Services.Interfaces
{
    using Fanex.NCB.Models.ViewLog;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IViewLogService
    {
        Task<(int, IEnumerable<ViewLogNotificationSystemMessage>)> GetNotificationSystemMessage(int? notificationId, int pageIndex, int pageSize);
        Task<(int, IEnumerable<ViewLogNotificationRecipientList>)> GetNotificationRecipientList(int? notificationId, int pageIndex, int pageSize);
        Task<(int, IEnumerable<ViewLogConversationMessageLog>)> GetConversationMessageLog(int? notificationId, int pageIndex, int pageSize);
    }
}
