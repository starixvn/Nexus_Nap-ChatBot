namespace Fanex.NCB.Models.Notification
{
    using System;
    using System.Collections.Generic;

    public class NotificationErrorReport
    {
        public string Title { get; set; }

        public DateTime ConfigSendTIme { get; set; }

        public string ResolvedMark { get; set; }

        public string ResolvedByUserName { get; set; }

        public IEnumerable<string> SuccessRecipients { get; set; }

        public IEnumerable<NotificationErrorDetail> ErrorDetails { get; set; }
    }
}