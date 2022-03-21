namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetEditNotificationRecipientsCriteria : CriteriaBase
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string Platforms { get; set; }

        public string RecipientIds { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Recipient_Sel";

        public override bool IsValid() => NotificationTypeId > 0 && NotificationId > 0;
    }
}