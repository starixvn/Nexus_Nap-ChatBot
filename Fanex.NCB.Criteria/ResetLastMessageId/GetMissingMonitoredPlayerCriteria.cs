namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetMissingMonitoredPlayerCriteria : CriteriaBase
{
    public string FromLogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MissingMonitoredPlayer_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}