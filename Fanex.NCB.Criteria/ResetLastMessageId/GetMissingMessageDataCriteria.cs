namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.Data.Repository;

    public class GetMissingMessageDataCriteria : CriteriaBase
    {
        public string MessageList { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMessage_Sel";

        public override bool IsValid() => true;
    }
}
