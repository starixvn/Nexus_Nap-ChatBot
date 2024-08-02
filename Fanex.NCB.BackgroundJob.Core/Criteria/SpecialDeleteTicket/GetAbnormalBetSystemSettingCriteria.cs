namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalBetSystemSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_MsgSetting_SpecialDelete_AbnormalBets_Sel";

        public override bool IsValid() => true;
    }
}