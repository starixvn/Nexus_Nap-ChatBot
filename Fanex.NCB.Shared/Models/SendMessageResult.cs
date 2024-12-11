namespace Fanex.NCB.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public class SendMessageResult
    {
        public int NotificationId { get; set; }

        public string ConversationId { get; set; }

        public DateTime SentTime { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsIgnoreRetry { get; set; }
        public List<string> FailedAttachments { get; set; } = new List<string>();
    }
}