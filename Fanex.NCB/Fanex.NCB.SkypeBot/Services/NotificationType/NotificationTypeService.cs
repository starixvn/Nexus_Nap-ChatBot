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
        public List<NotificationTypeItem> GetAll()
        {
            return _notificationTypeStorage.GetAll();
          
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