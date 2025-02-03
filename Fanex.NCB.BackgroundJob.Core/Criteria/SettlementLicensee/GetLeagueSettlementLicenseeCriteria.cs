namespace Fanex.NCB.AutoJobs.Core.Criteria.SettlementLicensee;

using Fanex.Data.Repository;

public class GetLeagueSettlementLicenseeCriteria : CriteriaBase
{
    public string Message { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_Settlement_Licensee_LeagueSel";
    }

    public override bool IsValid()
    {
        return true;
    }
}