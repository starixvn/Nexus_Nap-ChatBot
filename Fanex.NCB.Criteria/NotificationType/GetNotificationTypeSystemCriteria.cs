namespace Fanex.NCB.Criteria.NotificationType
{
    using Data.Repository;

    public class GetNotificationTypeSystemCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_System_Sel";

        public override bool IsValid() => true;
    }
}