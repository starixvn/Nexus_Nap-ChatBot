namespace Fanex.NCB.Criteria.SettingConfiguration;

using Data.Repository;

public class GetMappingsCriteria : CriteriaBase
{
    public int JobId { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int Total { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_MappingList_Sel";
    }

    public override bool IsValid()
    {
        return JobId > 0 && PageIndex > 0 && PageSize > 0;
    }
}