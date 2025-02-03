namespace Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;

using Fanex.Data.Repository;

public class GetSiteMessageSettingCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_MsgSetting_SiteMessage_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}