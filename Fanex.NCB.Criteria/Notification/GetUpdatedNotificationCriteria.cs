namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetUpdatedNotificationCriteria : CriteriaBase
    {
        [SpParam(AllowNull = true)]
        public string NotificationIds { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_UpdatedNotification_Sel";

        public override bool IsValid() => true;
    }
}