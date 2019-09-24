namespace Fanex.NCB.Models.Notification
{
    using System;

    public class Notification
    {
        public int NotificationId { get; set; }

        public string NotificationTypeName { get; set; }

        public int CreatorId { get; set; }

        public string CreatorName { get; set; }

        public DateTime ConfigSendTime { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }

        public string RecipientSentStatusJson { get; set; }
    }
}