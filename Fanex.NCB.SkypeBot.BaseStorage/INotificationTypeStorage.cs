using System.Collections.Generic;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface INotificationTypeStorage
    {
        List<NotificationTypeItem> GetAll();

        List<RecipientType> GetRecipientTypes();

        List<NotificationSystem> GetNotificationSystems();
    }
}
