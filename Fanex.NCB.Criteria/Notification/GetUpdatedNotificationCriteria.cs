namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetUpdatedNotificationCriteria : CriteriaBase
    {
        public string NotificationIds { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_UpdatedNotification_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NotificationIds);
    }
}