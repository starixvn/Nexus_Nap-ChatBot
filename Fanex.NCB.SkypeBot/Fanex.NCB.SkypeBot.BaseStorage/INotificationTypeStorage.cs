using System.Collections.Generic;
using Fanex.NCB.SkypeBot.Model.DTO;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface INotificationTypeStorage
    {
        NotificationTypeListResultModel GetNotificationTypeList(int pageIndex, int pageSize);

        List<RecipientType> GetRecipientTypes();

        List<NotificationSystem> GetNotificationSystems();

        ResultModel CreateNotificationType(NotificationTypeItem item);

        ResultModel UpdateNotificationType(NotificationTypeItem item);

        ResultModel DeleteNotificationType(int notificationTypeId);
    }
}
