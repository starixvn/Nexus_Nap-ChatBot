using System.Collections.Generic;
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

        public NotificationTypeListResultModel GetNotificationTypeList(int pageIndex, int pageSize)
        {
            return _notificationTypeStorage.GetNotificationTypeList(pageIndex, pageSize);
          
        }

        public List<NotificationSystem> GetNotificationSystems()
        {
            return _notificationTypeStorage.GetNotificationSystems();
        }

        public List<RecipientType> GetRecipientTypes()
        {
            return _notificationTypeStorage.GetRecipientTypes();
        }

        public ResultModel CreateNotificationType(NotificationTypeItem item)
        {
            return _notificationTypeStorage.CreateNotificationType(item);
        }

        public ResultModel UpdateNotificationType(NotificationTypeItem item)
        {
            return _notificationTypeStorage.UpdateNotificationType(item);
        }

        public ResultModel DeleteNotificationType(int notificationTypeId)
        {
            return _notificationTypeStorage.DeleteNotificationType(notificationTypeId);
        }
    }
}