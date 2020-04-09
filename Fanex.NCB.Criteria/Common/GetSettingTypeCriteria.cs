namespace Fanex.NCB.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetSettingTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Common_SettingType_Sel";

        public override bool IsValid() => true;
    }
}