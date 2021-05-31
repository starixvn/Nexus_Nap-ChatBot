namespace Fanex.NCB.Models.Notification
{
    using System.Collections.Generic;
    using Fanex.NCB.Models.Common;

    public class ResendNotification : ConversationChannel
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<Recipient> Recipients { get; set; }
    }
}