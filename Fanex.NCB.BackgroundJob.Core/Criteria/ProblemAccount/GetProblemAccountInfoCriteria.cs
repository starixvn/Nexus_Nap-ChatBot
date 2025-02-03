namespace Fanex.NCB.AutoJobs.Core.Criteria.ProblemAccount;

using Fanex.Data.Repository;

public class GetProblemAccountInfoCriteria : CriteriaBase
{
    public int BatchSize { get; set; }

    public int LastLogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_ProblemAccounts_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}