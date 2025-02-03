namespace Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;

using System.Collections.Generic;

public class NotificationMessage
{
    public int NotificationId { get; set; }
    public string Message { get; set; }
    public byte ChannelId { get; set; }
        public List<string> Attachments { get; set; }

    public IEnumerable<ConversationInfo> Recipients { get; set; }
}