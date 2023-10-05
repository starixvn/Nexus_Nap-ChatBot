namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetResendNotificationInfoCriteria : CriteriaBase
    {
        public int InNotificationId { get; set; }

        public bool InIsSendAll { get; set; }

        public override string GetSettingKey() => "NCB_ResendNotification_Sel";

        public override bool IsValid() => InNotificationId > 0;
    }
}