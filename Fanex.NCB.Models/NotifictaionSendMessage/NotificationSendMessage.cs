namespace Fanex.NCB.Models.NotifictaionSendMessage
{
    using System.Collections.Generic;

    public class NotificationSendMessage
    {
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<NotificationRecipient> Recipients { get; set; }
    }
}