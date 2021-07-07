namespace Fanex.NCB.Models.ViewLog
{
    using System;

    public class ViewLogNotificationSystemMessage
    {
        public int MessageID { get; set; }

        public int NotificationID { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CheckMsg { get; set; }
    }
}
