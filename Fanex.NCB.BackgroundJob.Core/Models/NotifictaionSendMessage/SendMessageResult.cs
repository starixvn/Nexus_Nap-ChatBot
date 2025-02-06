namespace Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage
{
    using System;

    public class SendMessageResult
    {
        public int NotificationId { get; set; }
        public string ConversationId { get; set; }
        public DateTime SentTime { get; set; }
        public int ErrorCode { get; set; }
        public string FailedImages { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsIgnoreRetry { get; set; }
    }
}