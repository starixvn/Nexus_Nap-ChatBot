namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEditNotificationRecipientsCriteria : CriteriaBase
    {
        public int InNotificationId { get; set; }

        public int InNotificationTypeId { get; set; }

        [SpParam(AllowNull = true)]
        public int? InProductId { get; set; }

        [SpParam(AllowNull = true)]
        public string InPlatformIds { get; set; }

        [SpParam(AllowNull = true)]
        public string InRecipientIds { get; set; }

        [SpParam(AllowNull = true)]
        public string InIntegrationMethodIds { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Recipient_Sel";

        public override bool IsValid() => InNotificationTypeId > 0 && InNotificationId > 0;
    }
}