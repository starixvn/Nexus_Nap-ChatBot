namespace Fanex.NCB.Models.NotifictaionSendMessage
{
    using Fanex.NCB.Core.Enum;

    public class NotificationRecipient
    {
        public int NotificationId { get; set; }
        public int RecipientId { get; set; }
        public string RecipientName { get; set; }
        public int ChatGroupId { get; set; }
        public string ChatGroupConversationId { get; set; }
        public int BotId { get; set; }
        public string BotName { get; set; }
        public string BotConversationId { get; set; }
        public byte ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ServiceUrl { get; set; }
        public string SiteNameGroup { get; set; }
        public int ErrCode { get; set; }
        public string ErrorMessage { get; set; }
        public RecipientStatuses RecipientStatus => (RecipientStatuses)ErrCode;
        public string FailedImages { get; set; }
    }
}