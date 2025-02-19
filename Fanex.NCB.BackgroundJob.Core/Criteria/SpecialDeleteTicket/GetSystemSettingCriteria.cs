namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetSystemSettingCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_MsgSetting_SpecialDelete_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}