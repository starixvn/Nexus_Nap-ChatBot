namespace Fanex.NCB.Criteria.NotificationType
{
    public class InsertNotificationTypeCommand : BaseCommand
    {
        public string NotificationTypeName { get; set; }

        public string RecipientTypeIds { get; set; }

        public int SystemId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Ins";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NotificationTypeName) &&
                                SystemId > 0 && !string.IsNullOrWhiteSpace(RecipientTypeIds);
    }
}