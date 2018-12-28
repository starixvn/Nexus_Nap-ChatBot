using System.Collections.Generic;
using Fanex.NCB.SkypeBot.Models.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public interface INotificationTypeService
    {
        List<NotificationTypeItem> GetAll();
    }
}