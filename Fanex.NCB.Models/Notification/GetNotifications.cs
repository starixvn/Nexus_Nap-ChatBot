namespace Fanex.NCB.Models.Notification
{
    using System;

    public class GetNotifications
    {
        public string NotificationTypeIds { get; set; }

        public string RecipientIds { get; set; }

        public int? CreatorId { get; set; }

        public string NotificationStatusIds { get; set; }
        public string ChannelIds { get; set; }
        public uint? MessageId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}