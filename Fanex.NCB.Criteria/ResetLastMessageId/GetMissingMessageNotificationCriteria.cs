namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetMissingMessageNotificationCriteria : CriteriaBase
{
    public string Setting { get; set; }

    public string MessageList { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MissingNotification_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}