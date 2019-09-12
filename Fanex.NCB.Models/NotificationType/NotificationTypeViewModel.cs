using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Models.NotificationType
{
    public class NotificationTypeViewModel
    {
        public int NotificationTypeId { get; set; }

        public string NotificationTypeName { get; set; }

        public string RecipientTypeIds { get; set; }

        public string ChatGroupTypeIds { get; set; }                

        public int SystemId { get; set; }
    }
}
