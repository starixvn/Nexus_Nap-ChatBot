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
                },
                new NotificationTypeItem
                {
                    NotificationTypeId = 4,
                    NotificationTypeName = "Type 4",
                    RecipientTypeId = 4,
                    RecipientTypeName = "Licensee",
                    SystemId = 4,
                    SystemName = "Admin Site"
                },
                new NotificationTypeItem
                {
                    NotificationTypeId = 5,
                    NotificationTypeName = "Type 5",
                    RecipientTypeId = 5,
                    RecipientTypeName = "Licensee",
                    SystemId = 5,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 6,
                    NotificationTypeName = "Type 6",
                    RecipientTypeId = 6,
                    RecipientTypeName = "Licensee",
                    SystemId = 6,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 7,
                    NotificationTypeName = "Type 7",
                    RecipientTypeId = 7,
                    RecipientTypeName = "Licensee",
                    SystemId = 7,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 8,
                    NotificationTypeName = "Type 8",
                    RecipientTypeId = 8,
                    RecipientTypeName = "Licensee",
                    SystemId = 8,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 9,
                    NotificationTypeName = "Type 9",
                    RecipientTypeId = 9,
                    RecipientTypeName = "Licensee",
                    SystemId = 9,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 10,
                    NotificationTypeName = "Type 10",
                    RecipientTypeId = 10,
                    RecipientTypeName = "Licensee",
                    SystemId = 10,
                    SystemName = "Admin Site"
                },
                 new NotificationTypeItem
                {
                    NotificationTypeId = 11,
                    NotificationTypeName = "Type 11",
                    RecipientTypeId = 11,
                    RecipientTypeName = "Licensee",
                    SystemId = 11,
                    SystemName = "Admin Site"
                }
            };
        }

        public List<NotificationSystem> GetNotificationSystems()
        {
            return new List<NotificationSystem>
            {
                new NotificationSystem {
                    Id = 1,
                    Name = "Bot site"
                },
                new NotificationSystem {
                    Id = 2,
                    Name = "Admin site"
                },
                new NotificationSystem {
                    Id = 3,
                    Name = "Special Delete site"
                }
            };
        }

        public List<RecipientType> GetRecipientTypes()
        {
            return new List<RecipientType>{
                new RecipientType {
                    Id = 1,
                    Name = "Licensee"
                },
                new RecipientType {
                    Id = 2,
                    Name = "3rd party"
                }
            };
        }
    }
}
