namespace Fanex.NCB.Criteria.SettingConfiguration;

using Data.Repository;

public class GetJobInfoCriteria : CriteriaBase
{
    public int JobId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_JobInfo_Sel";
    }

    public override bool IsValid()
    {
        return JobId > 0;
    }
}