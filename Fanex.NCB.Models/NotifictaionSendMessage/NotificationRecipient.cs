using Fanex.NCB.Core.Enum;

namespace Fanex.NCB.Models.NotifictaionSendMessage
{
    public class NotificationRecipient
    {
        public int NotificationId { get; set; }

        public int ChatGroupId { get; set; }

        public string ChatGroupConversation { get; set; }

        public string BotName { get; set; }

        public string ChannelId { get; set; }

        public string ServiceUrl { get; set; }

        public string BotConversationId { get; set; }

        public int SendingError { get; set; }

        public RecipientStatuses RecipientStatus
        {
            get
            {
                return (RecipientStatuses)SendingError;
            }
        }
    }
}