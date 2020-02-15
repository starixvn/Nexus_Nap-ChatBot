namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalBetInfoCriteria : CriteriaBase
    {
        public string MessageIdLst { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_AbnormalBets_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageIdLst);
    }
}