namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRecipientsCriteria : CriteriaBase
    {
        public int InNotificationTypeId { get; set; }

        public int? InProductId { get; set; }

        public string InIntegrationMethodIds { get; set; }

        public string InChannelIds { get; set; }

        public string InPlatformIds { get; set; }

        public string InRecipientIds { get; set; }

        public override string GetSettingKey() => "NCB_NewNotification_Recipient_Sel";

        public override bool IsValid() => InNotificationTypeId > 0;
    }
}