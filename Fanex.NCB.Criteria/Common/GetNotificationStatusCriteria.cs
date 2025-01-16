namespace Fanex.NCB.Criteria.Common
{
    using Data.Repository;

    public class GetNotificationStatusCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_NotificationStatus_Sel";

        public override bool IsValid() => true;
    }
}