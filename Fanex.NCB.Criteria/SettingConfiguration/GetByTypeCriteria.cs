namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.Data.Repository;

    public class GetByTypeCriteria : CriteriaBase
    {
        public int Type { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_SettingType_Sel";

        public override bool IsValid() => Type > 0;
    }
}