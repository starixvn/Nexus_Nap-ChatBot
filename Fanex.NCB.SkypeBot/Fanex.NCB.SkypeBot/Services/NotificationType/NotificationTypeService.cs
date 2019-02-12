using System.Collections.Generic;
using Fanex.NCB.SkypeBot.BaseStorage;
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
    }
}