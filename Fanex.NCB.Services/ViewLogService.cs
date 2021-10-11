namespace Fanex.NCB.Services
{
    using Fanex.NCB.Criteria.ViewLog;
    using Fanex.NCB.Models.ViewLog;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
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

        public async Task<(int, IEnumerable<ViewLogNotificationSystemMessage>)> GetNotificationSystemMessageAsync(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, systemMessages) = await baseService.FetchMultipleAsync<int, ViewLogNotificationSystemMessage>(new GetNotificationSystemMessageCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (totalItems.FirstOrDefault(), systemMessages);
        }

        public async Task<(int, IEnumerable<ViewLogNotificationRecipientList>)> GetNotificationRecipientListAsync(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, recipients) = await baseService.FetchMultipleAsync<int, ViewLogNotificationRecipientList>(new GetNotificationRecipientListCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (totalItems.FirstOrDefault(), recipients);
        }

        public async Task<(int, IEnumerable<ViewLogConversationMessageLog>, IEnumerable<ViewLogConversationMessageLogRecipient>)>
        GetConversationMessageLogAsync(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, conversationMessages, mapRecipient) = await baseService.FetchMultipleAsync<int, ViewLogConversationMessageLog, ViewLogConversationMessageLogRecipient>
            (
                new GetConversationMessageLogCriteria
                {
                    NotificationId = notificationId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            );

            return (totalItems.FirstOrDefault(), conversationMessages, mapRecipient);
        }

        public async Task<CommandResult> DeleteOldConversationMessageLogAsync(string currentDate)
        {
            var command = new DeleteOldConversationMessageLogCommand
            {
                CurrentDate = currentDate,
            };

            return await baseService.ExecuteNonQueryAsync(command);                      
        }
    }
}
