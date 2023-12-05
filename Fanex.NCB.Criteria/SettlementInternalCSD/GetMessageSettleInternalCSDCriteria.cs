namespace Fanex.NCB.Criteria.SettlementInternalCSD
{
    using Fanex.Data.Repository;

    public class GetMessageSettleInternalCSDCriteria : CriteriaBase
    {
        public int BatchSize { get; set; }

        public int LastMsgTargetId { get; set; }

        public int MaxMsgTargetId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_Settlement_InternalCSD_MessageSel";

        public override bool IsValid() => true;
    }
}
