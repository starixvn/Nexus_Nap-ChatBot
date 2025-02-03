namespace Fanex.NCB.Criteria.SettingConfiguration;

using Data.Repository;

public class GetAllSettingTypeCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_MappingSettingTypeList_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}