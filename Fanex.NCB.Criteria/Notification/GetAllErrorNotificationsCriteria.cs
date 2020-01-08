namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllErrorNotificationsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "notificationTypeIDLst")]
        public string NotificationTypeIds { get; set; }

        [SpParam(SqlParamName = "recipientIDLst")]
        public string RecipientIds { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_GetErrNotifications_Sel";

        public override bool IsValid() => true;
    }
}