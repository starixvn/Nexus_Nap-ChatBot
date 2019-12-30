namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetAllErrorNotificationsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_NotificationList_GetErrNotifications_Sel";

        public override bool IsValid() => true;
    }
}