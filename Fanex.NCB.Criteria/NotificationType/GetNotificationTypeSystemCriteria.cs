namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.Data.Repository;

    public class GetNotificationTypeSystemCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_System_Sel";

        public override bool IsValid() => true;
    }
}