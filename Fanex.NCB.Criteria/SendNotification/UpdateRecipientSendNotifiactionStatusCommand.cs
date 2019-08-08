namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.NCB.Shared.Models;

    public class UpdateRecipientSendNotifiactionStatusCommand : BaseCommand
    {
        public int NotificationId { get; set; }

        public string RecipientConversationId { get; set; }

        public int SendNotifiactionStatus { get; set; }

        public override string GetSettingKey() => "NCB_SendNotification_UpdateRecipientSendNotiStatus";

        public override bool IsValid() => NotificationId > 0 && !string.IsNullOrWhiteSpace(RecipientConversationId);
    }
}