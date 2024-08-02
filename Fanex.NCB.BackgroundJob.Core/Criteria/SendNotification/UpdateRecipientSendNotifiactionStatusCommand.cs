namespace Fanex.NCB.AutoJobs.Core.SendNotification
{
    using Fanex.NCB.AutoJobs.Core.Criteria;

    public class UpdateRecipientSendNotifiactionStatusCommand : BaseCommand
    {
        public string SentStatus { get; set; }

        public string UpdatedNotificationIds { get; set; }

        public bool IsHaveErrNotification { get; set; }

        public override string GetSettingKey() => "NCB_SendNotification_RecipientSentStatus_Upd";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(SentStatus);
    }
}