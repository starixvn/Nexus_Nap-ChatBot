namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetMessageFromSpecialDeleteCriteria : CriteriaBase
    {
        public int MessageID { get; set; }

        public int BatchSize { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_GetSpecialDeleteMsg";

        public override bool IsValid() => MessageID > 0 && BatchSize > 0;
    }
}