namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.NCB.Shared.Models;

    public class DeleteNotificationTypeCommand : BaseCommand
    {
        public int NotificationId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Del";

        public override bool IsValid() => NotificationId > 0;
    }
}