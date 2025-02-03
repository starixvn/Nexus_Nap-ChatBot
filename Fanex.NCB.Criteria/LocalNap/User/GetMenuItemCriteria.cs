namespace Fanex.NCB.Criteria.LocalNap.User;

using Data.Repository;

public class GetMenuItemCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int SiteId { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_MenuItem_GetByUserAndSiteIncludeGroup";
    }

    public override bool IsValid()
    {
        return UserId > 0 && SiteId > 0;
    }
}