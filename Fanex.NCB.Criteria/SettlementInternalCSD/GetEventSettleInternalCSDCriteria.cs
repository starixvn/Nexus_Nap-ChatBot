namespace Fanex.NCB.Criteria.SettlementInternalCSD
{
    using Fanex.Data.Repository;

    public class GetEventSettleInternalCSDCriteria : CriteriaBase
    {
        public string MessageInfo { get; set; }

        public string MessageDetail { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_Settlement_InternalCSD_EventInfoSel";

        public override bool IsValid() => true;
    }
}
