namespace Fanex.NCB.Main.ViewModel.Notification
{
    using System;

    public class GetSignalRNotificationUpdatedViewModel
    {
        public string NotificationIds { get; set; }

        public string NotificationTypeIds { get; set; }

        public string RecipientIds { get; set; }

        public int? CreatorId { get; set; }

        public string NotificationStatusIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}