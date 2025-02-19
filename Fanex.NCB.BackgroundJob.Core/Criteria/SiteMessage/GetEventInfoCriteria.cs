namespace Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;

using Fanex.Data.Repository;

public class GetEventInfoCriteria : CriteriaBase
{
    public string MatchIDLst { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_SiteMesssage_EventInfo_Sel";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MatchIDLst);
    }
}