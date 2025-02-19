namespace Fanex.NCB.AutoJobs.Core.Criteria.ProblemAccount;

using Fanex.Data.Repository;

public class ImportFromProblemAccountCriteria : CriteriaBase
{
    public string Message { get; set; }

    public string CategoryInfo { get; set; }

    public string CustomerClassInfo { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ProblemAccount_Import";
    }

    public override bool IsValid()
    {
        return true;
    }
}