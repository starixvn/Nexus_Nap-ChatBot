namespace Fanex.NCB.Criteria.ResetLastMessageId;

using Data.Repository;

public class GetMissingMonitoringMatchCriteria : CriteriaBase
{
    public string FromLogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MissingMatchMonitoring_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}