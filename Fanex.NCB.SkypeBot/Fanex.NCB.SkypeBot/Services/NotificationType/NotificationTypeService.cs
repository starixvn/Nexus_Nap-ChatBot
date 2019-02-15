using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public class NotificationTypeService : INotificationTypeService
    {
        private readonly INotificationTypeStorage _notificationTypeStorage;
        public NotificationTypeService(INotificationTypeStorage notificationTypeStorage)
        {
            _notificationTypeStorage = notificationTypeStorage;
        }

        public async Task<NotificationTypeListResultModel> GetNotificationTypeListAsync(int pageIndex, int pageSize)
        {
            return await _notificationTypeStorage.GetNotificationTypeListAsync(pageIndex, pageSize);          
        }

        public async Task<List<NotificationSystem>> GetNotificationSystemsAsync()
        {
            return await _notificationTypeStorage.GetNotificationSystemsAsync();
        }

        public async Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return await _notificationTypeStorage.GetRecipientTypesAsync();
        }

        public async Task<ResultModel> CreateNotificationTypeAsync(NotificationTypeItem item)
        {
            return await _notificationTypeStorage.CreateNotificationTypeAsync(item);
        }

        public async Task<ResultModel> UpdateNotificationTypeAsync(NotificationTypeItem item)
        {
            return await _notificationTypeStorage.UpdateNotificationTypeAsync(item);
        }

        public async Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId)
        {
            return await _notificationTypeStorage.DeleteNotificationTypeAsync(notificationTypeId);
        }
    }
}