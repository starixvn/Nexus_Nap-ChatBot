namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalAccountUsernameLstCriteria : CriteriaBase
    {
        public string MessageIDLst { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_AbnormalBets_GetUsernameLst";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageIDLst);
    }
}