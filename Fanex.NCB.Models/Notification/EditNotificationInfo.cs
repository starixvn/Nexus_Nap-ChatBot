namespace Fanex.NCB.Models.Notification
{
    using System;
    using System.Collections.Generic;

    public class EditNotificationInfo
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }

        public string NotificationTypeName { get; set; }

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        public DateTime ConfigSendTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool SendImmediately { get; set; }

        public IEnumerable<int> RecipientIds { get; set; }
    }
}