using System.Collections.Generic;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.MySQLStorage
{
    public class NotificationTypeStorage : INotificationTypeStorage
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

        public List<NotificationSystem> GetNotificationSystems()
        {
            return new List<NotificationSystem>
            {
                new NotificationSystem {
                    SystemId = 1,
                    SystemName = "Bot site"
                },
                new NotificationSystem {
                    SystemId = 2,
                    SystemName = "Admin site"
                },
                new NotificationSystem {
                    SystemId = 3,
                    SystemName = "Special Delete site"
                }
            };
        }

        public List<RecipientType> GetRecipientTypes()
        {
            return new List<RecipientType>{
                new RecipientType {
                    RecipientTypeId = 1,
                    RecipientTypeName = "Licensee"
                },
                new RecipientType {
                    RecipientTypeId = 2,
                    RecipientTypeName = "3rd party"
                }
            };
        }
    }
}
