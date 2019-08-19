namespace Fanex.NCB.Models.Notification
{
    using System;

    public class EditNotification
    {
        public int NotificationId { get; set; }

        public string RecipientIds { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime SendDate { get; set; }

        public bool SendImmediately { get; set; }

        public int UserId { get; set; }
    }
}