namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetEditNotificationInfoCriteria : CriteriaBase
    {
        public int InNotificationId { get; set; }

        public override string GetSettingKey() => "NCB_EditNotification_Sel";

        public override bool IsValid() => InNotificationId > 0;
    }
}