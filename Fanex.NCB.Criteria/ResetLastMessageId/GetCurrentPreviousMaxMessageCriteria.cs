namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.Data.Repository;

    public class GetCurrentPreviousMaxMessageCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_JobInfo_Sel";

        public override bool IsValid() => true;
    }
}
