namespace Fanex.NCB.Criteria.Common
{
    using Data.Repository;

    public class GetNotificationTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_NotificationType_Sel";

        public override bool IsValid() => true;
    }
}