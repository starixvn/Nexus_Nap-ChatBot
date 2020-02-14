namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalBetMessageCriteria : CriteriaBase
    {
        public int MaxMessageId { get; set; }

        public int BatchSize { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_AbnormalBet_Sel";

        public override bool IsValid() => MaxMessageId > 0 && BatchSize > 0;
    }
}