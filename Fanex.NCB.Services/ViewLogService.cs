namespace Fanex.NCB.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using Criteria.ViewLog;
using Models.ViewLog;
using Interfaces;
using Fanex.NCB.Shared.Services.Interfaces;

public class ViewLogService : IViewLogService
{
    private readonly IBaseService baseService;

    public ViewLogService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<(int, IEnumerable<ViewLogNotificationSystemMessage>)> GetNotificationSystemMessageAsync(
        int? notificationId, int pageIndex, int pageSize)
    {
        var (totalItems, systemMessages) = await baseService.FetchMultipleAsync<int, ViewLogNotificationSystemMessage>(
            new GetNotificationSystemMessageCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        foreach (var systemMessage in systemMessages)
        {
            systemMessage.CreatedDate = systemMessage.CreatedDate.ConvertFromUTC();
        }

        return (totalItems.FirstOrDefault(), systemMessages);
    }

    public async Task<(int, IEnumerable<ViewLogNotificationRecipientList>)> GetNotificationRecipientListAsync(
        int? notificationId, int pageIndex, int pageSize)
    {
        var (totalItems, recipients) = await baseService.FetchMultipleAsync<int, ViewLogNotificationRecipientList>(
            new GetNotificationRecipientListCriteria
            {
                NotificationId = notificationId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        foreach (var recipient in recipients)
        {
            recipient.SentTime = recipient.SentTime.ConvertFromUTC();
        }

        return (totalItems.FirstOrDefault(), recipients);
    }

    public async Task<(int, IEnumerable<ViewLogConversationMessageLog>,
            IEnumerable<ViewLogConversationMessageLogRecipient>)>
        GetConversationMessageLogAsync(int? notificationId, int pageIndex, int pageSize)
    {
        var (totalItems, conversationMessages, mapRecipient) = await baseService
            .FetchMultipleAsync<int, ViewLogConversationMessageLog, ViewLogConversationMessageLogRecipient>
            (
                new GetConversationMessageLogCriteria
                {
                    NotificationId = notificationId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            );
        foreach (var conversationMessage in conversationMessages)
        {
            conversationMessage.SentTime = conversationMessage.SentTime.ConvertFromUTC();
        }

        return (totalItems.FirstOrDefault(), conversationMessages, mapRecipient);
    }
}