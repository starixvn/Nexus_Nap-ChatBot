namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetCurrentPreviousMaxMessageCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_JobInfo_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}