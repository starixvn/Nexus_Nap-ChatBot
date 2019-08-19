namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.NCB.Shared.Models;
    using System;

    public class EditNotificationCommand : BaseCommand
    {
        public int NotificationId { get; set; }

        public string RecipientIDLst { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime ConfigSendTime { get; set; }

        public bool SendImmediately { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Upd";

#pragma warning disable S1067 // Expressions should not be too complex

        public override bool IsValid() => NotificationId > 0 && !string.IsNullOrWhiteSpace(RecipientIDLst) && !string.IsNullOrWhiteSpace(Title)
             && !string.IsNullOrWhiteSpace(Content) && UserId > 0;

#pragma warning restore S1067 // Expressions should not be too complex
    }
}