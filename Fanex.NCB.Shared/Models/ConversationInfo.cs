using System.Collections.Generic;

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
        public List<string> FailedImages { get; set; }
        public List<string> Attachments { get; set; }
}