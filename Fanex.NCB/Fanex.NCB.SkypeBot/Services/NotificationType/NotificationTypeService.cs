using System.Collections.Generic;
using Fanex.NCB.SkypeBot.Models.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public class NotificationTypeService : INotificationTypeService
    {
        public List<NotificationTypeItem> GetAll()
        {
            return new List<NotificationTypeItem>
            {
                new NotificationTypeItem 
                {
                    NotificationTypeId = 1,
                    NotificationTypeName = "UM",
                    RecipientTypeId = 1,
                    RecipientTypeName = "Licensee",
                    SystemId = 1,
                    SystemName = "I, Bot Service"
                },
                new NotificationTypeItem 
                {
                    NotificationTypeId = 2,
                    NotificationTypeName = "UM 3rd",
                    RecipientTypeId = 2,
                    RecipientTypeName = "3rd party",
                    SystemId = 1,
                    SystemName = "I, Bot Service"
                },
                new NotificationTypeItem 
                {
                    NotificationTypeId = 2,
                    NotificationTypeName = "Refund/Reject match message",
                    RecipientTypeId = 1,
                    RecipientTypeName = "Licensee",
                    SystemId = 2,
                    SystemName = "Admin Site"
                }
            };
        }
    }
}