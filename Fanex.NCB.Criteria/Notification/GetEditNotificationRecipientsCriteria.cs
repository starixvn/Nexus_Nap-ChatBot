namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetEditNotificationRecipientsCriteria : CriteriaBase
    {
        public int InNotificationId { get; set; }

        public int InNotificationTypeId { get; set; }

        public int? InProductId { get; set; }

        public string InPlatformIds { get; set; }

        public string InRecipientIds { get; set; }

        public string InIntegrationMethodIds { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Recipient_Sel";

        public override bool IsValid() => InNotificationTypeId > 0 && InNotificationId > 0;
    }
}