namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Data.Repository;

    public class GetAllSettingTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_SettingConfiguration_MappingSettingTypeList_Sel";

        public override bool IsValid() => true;
    }
}