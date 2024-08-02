namespace Fanex.NCB.AutoJobs.Core.Criteria.ProblemAccount
{
    using Fanex.Data.Repository;

    public class GetMessageSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_MsgSetting_ProblemAccounts";

        public override bool IsValid() => true;
    }
}