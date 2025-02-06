namespace Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage
{
    using System;

    [Serializable]
    public class UpdateNotificationModel
    {
        public int NotificationID { get; set; }

        public int RecipientID { get; set; }

        public int ConversationGroupID { get; set; }

        public int ConversationBotID { get; set; }

        public string SentTime { get; set; }

        public bool IsSuccess { get; set; }

        public bool Retry { get; set; }

        public string ErrMsg { get; set; }

        public string FailedImages { get; set; }
    }
}