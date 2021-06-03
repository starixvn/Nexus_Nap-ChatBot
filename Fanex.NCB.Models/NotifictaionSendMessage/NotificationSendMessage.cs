namespace Fanex.NCB.Models.NotifictaionSendMessage
{
    public class NotificationSendMessage
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public byte ChannelId { get; set; }
    }
}