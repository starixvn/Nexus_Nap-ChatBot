using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public interface INotificationTypeService
    {
        Task<NotificationTypeListResultModel> GetNotificationTypeListAsync(int pageIndex, int pageSize);

        Task<List<RecipientType>> GetRecipientTypesAsync();

        Task<List<NotificationSystem>> GetNotificationSystemsAsync();

        Task<ResultModel> CreateNotificationTypeAsync(CreateNotificationTypeCriteria item);

        Task<ResultModel> UpdateNotificationTypeAsync(CreateNotificationTypeCriteria item);

        Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId);
    }
}