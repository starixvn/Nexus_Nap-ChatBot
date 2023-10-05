namespace Fanex.NCB.Criteria.Notification
{
    using System;
    using Fanex.NCB.Shared.Models;

    public class EditNotificationCommand : BaseCommand
    {
        public int InNotificationId { get; set; }

        public string InRecipientIds { get; set; }

        public string InTitle { get; set; }

        public string InContent { get; set; }

        public DateTimeOffset InConfigSendTime { get; set; }

        public bool InSendImmediately { get; set; }

        public int UserId { get; set; }

        public string InPlatformIds { get; set; }

        public string InIntegrationMethodIds { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Upd";

#pragma warning disable S1067 // Expressions should not be too complex

        public override bool IsValid() => InNotificationId > 0 && !string.IsNullOrWhiteSpace(InRecipientIds) 
            && !string.IsNullOrWhiteSpace(InTitle)
             && !string.IsNullOrWhiteSpace(InContent) && UserId > 0;

#pragma warning restore S1067 // Expressions should not be too complex
    }
}