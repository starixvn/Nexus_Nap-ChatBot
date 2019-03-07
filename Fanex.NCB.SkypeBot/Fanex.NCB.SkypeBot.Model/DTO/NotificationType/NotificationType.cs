using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.DTO.NotificationType
{
    public class NotificationType
    {
        public int NotificationTypeId { get; set; }
        public string NotificationTypeName { get; set; }
        public List<RecipientType> RecipientTypes { get; set; }
        public int SystemId { get; set; }
        public string SystemName { get; set; }
    }
}
