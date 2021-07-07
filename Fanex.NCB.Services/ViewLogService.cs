namespace Fanex.NCB.Services
{
    using Fanex.NCB.Criteria.ViewLog;
    using Fanex.NCB.Models.ViewLog;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ViewLogService : IViewLogService
    {
        private readonly IBaseService baseService;

        public ViewLogService(IBaseService baseService)
        {
           this.baseService = baseService;
        }

        public async Task<(int, IEnumerable<ViewLogNotificationSystemMessage>)> GetNotificationSystemMessage(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<int, ViewLogNotificationSystemMessage>(new GetNotificationSystemMessageCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<(int, IEnumerable<ViewLogNotificationRecipientList>)> GetNotificationRecipientList(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<int, ViewLogNotificationRecipientList>(new GetNotificationRecipientListCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<(int, IEnumerable<ViewLogConversationMessageLog>)> GetConversationMessageLog(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<int, ViewLogConversationMessageLog>(new GetConversationMessageLogCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (result.Item1.FirstOrDefault(), result.Item2);
        }
    }
}
