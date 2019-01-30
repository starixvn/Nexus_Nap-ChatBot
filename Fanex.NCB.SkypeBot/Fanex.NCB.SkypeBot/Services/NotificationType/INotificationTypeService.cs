using System.Collections.Generic;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public interface INotificationTypeService
    {
        List<NotificationTypeItem> GetAll();

        List<RecipientType> GetRecipientTypes();

        List<NotificationSystem> GetNotificationSystems();
    }
}