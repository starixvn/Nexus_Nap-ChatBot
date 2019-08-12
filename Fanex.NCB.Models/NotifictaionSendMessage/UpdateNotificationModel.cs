namespace Fanex.NCB.Models.NotifictaionSendMessage
{
    using System;
    using Fanex.NCB.Core.Enum;

    public class UpdateNotificationModel
    {
        public int NotificationId { get; set; }

        public int RecipientId { get; set; }

        public int ConversationGroupId { get; set; }

        public int ConversationBotId { get; set; }

        public DateTime SentTime { get; set; }

        public bool IsSuccess { get; set; }

        public RecipientStatuses SentStatus { get; set; }

        public string ErrMsg { get; set; }
    }
}