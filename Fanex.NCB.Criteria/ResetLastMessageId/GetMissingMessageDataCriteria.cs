namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetMissingMessageDataCriteria : CriteriaBase
{
    public string MessageList { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MissingMessage_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}