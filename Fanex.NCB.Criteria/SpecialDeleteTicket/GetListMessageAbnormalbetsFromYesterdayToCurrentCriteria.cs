namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_AbnormalBets_GetMessage";

        public override bool IsValid() => true;
    }
}