namespace Fanex.NCB.Criteria.NotificationType
{
    public class UpdateNotificationTypeCommand : BaseCommand
    {
        public int NotificationTypeId { get; set; }

        public string NotificationTypeName { get; set; }

        public string RecipientTypeIds { get; set; }

        public int SystemId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Upd";

        public override bool IsValid() => NotificationTypeId > 0 && SystemId > 0 &&
                                !string.IsNullOrWhiteSpace(NotificationTypeName) &&
                                !string.IsNullOrWhiteSpace(RecipientTypeIds);
    }
}