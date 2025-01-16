namespace Fanex.NCB.Criteria.Notification
{
    using Data.Repository;

    public class GetNotificationErrorDetailCriteria : CriteriaBase
    {
        public int NotificationId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_ErrDetail_Sel";

        public override bool IsValid() => NotificationId > 0;
    }
}