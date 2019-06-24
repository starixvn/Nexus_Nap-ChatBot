using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface INotificationTypeStorage
    {
        Task<Tuple<int, List<NotificationTypeItem>>> GetNotificationTypeListAsync(int pageIndex, int pageSize);

        Task<List<RecipientType>> GetRecipientTypesAsync();

        Task<List<NotificationSystem>> GetNotificationSystemsAsync();

        Task<ResultModel> CreateNotificationTypeAsync(CreateNotificationTypeCriteria item);

        Task<ResultModel> UpdateNotificationTypeAsync(CreateNotificationTypeCriteria item);

        Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId);
    }
}