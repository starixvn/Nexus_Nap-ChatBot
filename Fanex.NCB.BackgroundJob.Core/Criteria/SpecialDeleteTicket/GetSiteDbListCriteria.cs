namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetSiteDbListCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "Acc_SiteDBConfigSel";
    }

    public override bool IsValid()
    {
        return true;
    }
}