namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetResendNotificationInfoCriteria : CriteriaBase
    {
        public int NotificationId { get; set; }

        public bool IsSendAll { get; set; }

        public override string GetSettingKey() => "NCB_ResendNotification_Sel";

        public override bool IsValid() => NotificationId > 0;
    }
}