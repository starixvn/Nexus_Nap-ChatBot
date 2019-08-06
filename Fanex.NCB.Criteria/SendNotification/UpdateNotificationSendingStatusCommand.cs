namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.NCB.Shared.Models;

    public class UpdateNotificationSendingStatusCommand : BaseCommand
    {
        public int NotificationId { get; set; }

        public int Creator { get; set; }

        public override string GetSettingKey() => "NCB_SendNotification_LockForSending_Upd";

        public override bool IsValid() => NotificationId > 0 && Creator > 0;
    }
}