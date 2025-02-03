namespace Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;

public class NotificationSendMessage
{
    public int NotificationId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public byte ChannelId { get; set; }
    public string ChannelName { get; set; }
    public string SiteNameGroup { get; set; }
        public string FileName { get; set; }
}