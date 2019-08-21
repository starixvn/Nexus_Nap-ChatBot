namespace Fanex.NCB.Models.Notification
{
    using System;
    using System.Collections.Generic;

    public class NotificationErrorReport
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }

        public DateTime ConfigSendTime { get; set; }

        public string ResolvedRemark { get; set; }

        public string ResolvedByUserName { get; set; }

        public int? ResolvedBy { get; set; }

        public int StatusId { get; set; }

        public IEnumerable<string> SuccessRecipients { get; set; }

        public IEnumerable<NotificationErrorDetail> ErrorDetails { get; set; }
    }
}