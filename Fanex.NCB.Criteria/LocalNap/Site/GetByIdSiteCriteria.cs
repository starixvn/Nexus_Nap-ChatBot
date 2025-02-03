namespace Fanex.NCB.Criteria.LocalNap.Site;

using Data.Repository;

public class GetByIdSiteCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetById";
    }

    public override bool IsValid()
    {
        return SiteId > 0;
    }
}