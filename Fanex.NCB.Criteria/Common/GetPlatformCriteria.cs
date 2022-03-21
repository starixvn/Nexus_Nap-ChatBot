namespace Fanex.NCB.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetPlatformCriteria : CriteriaBase
    {
        public int NotificationTypeId { get; set; }

        public override string GetSettingKey() => "NCB_Common_Platform_Sel";

        public override bool IsValid() => NotificationTypeId > 0;
    }
}
