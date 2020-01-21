using System.Collections.Generic;
using Fanex.NCB.Models.Common;

namespace Fanex.NCB.Models.Notification
{
    public class ResendNotification
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<Recipient> Recipients { get; set; }
    }
}