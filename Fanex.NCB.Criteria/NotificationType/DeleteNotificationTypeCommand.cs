namespace Fanex.NCB.Criteria.NotificationType
{
    public class DeleteNotificationTypeCommand : BaseCommand
    {
        public int NotificationId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Del";

        public override bool IsValid() => NotificationId > 0;
    }
}