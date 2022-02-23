namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class GetPlatformCriteria : CriteriaBase
    {
        public int NotificationTypeId { get; set; }

        public override string GetSettingKey() => "NCB_Setting_Platform_Sel";

        public override bool IsValid() => NotificationTypeId > 0;
    }
}
