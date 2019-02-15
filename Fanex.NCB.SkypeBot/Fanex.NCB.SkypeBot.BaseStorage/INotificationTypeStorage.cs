using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.DTO;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface INotificationTypeStorage
    {
        Task<NotificationTypeListResultModel> GetNotificationTypeListAsync(int pageIndex, int pageSize);

        Task<List<RecipientType>> GetRecipientTypesAsync();

        Task<List<NotificationSystem>> GetNotificationSystemsAsync();

        Task<ResultModel> CreateNotificationTypeAsync(NotificationTypeItem item);

        Task<ResultModel> UpdateNotificationTypeAsync(NotificationTypeItem item);

        Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId);
    }
}
