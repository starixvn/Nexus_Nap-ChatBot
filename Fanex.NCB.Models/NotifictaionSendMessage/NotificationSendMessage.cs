namespace Fanex.NCB.Models.NotifictaionSendMessage;

    using System.Collections.Generic;
    using System.Linq;

public class NotificationSendMessage
{
    public int NotificationId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public byte ChannelId { get; set; }
    public string ChannelName { get; set; }
    public string SiteNameGroup { get; set; }
        public string FileName { get; set; }

        public List<string> Attachments => GetAttachmentFromFileName();

        private List<string> GetAttachmentFromFileName()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                return FileName.Split(",").ToList();
            }
            return new List<string>();
        }
}