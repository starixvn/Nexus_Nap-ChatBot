using System.Collections.Generic;

namespace Fanex.NCB.Shared.Models
{
    public class ConversationInfo
    {
        public int NotificationId { get; set; }
        public string ConversationId { get; set; }
        public string ChannelId { get; set; }
        public string ServiceUrl { get; set; }
        public string BotConversationId { get; set; }
        public string BotName { get; set; }
        public string Message { get; set; }
        public bool IsDuplicated { get; set; }
        public bool RetryImageOnly => FailedAttachments is not null && FailedAttachments.Count > 0;
        public List<string> FailedAttachments { get; set; }
        public List<string> Attachments { get; set; }
    }
}