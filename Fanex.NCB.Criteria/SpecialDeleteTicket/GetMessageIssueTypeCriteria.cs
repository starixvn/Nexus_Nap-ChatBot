namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetMessageIssueTypeCriteria : CriteriaBase
    {
        public string Message { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDeleteIssueType_Sel";

        public override bool IsValid() => !string.IsNullOrEmpty(Message);
    }
}