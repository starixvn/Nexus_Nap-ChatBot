namespace Fanex.NCB.AutoJobs.Core.Criteria.MonitorMatch;

using Fanex.Data.Repository;

public class GetMsgSettingMonitorMatchCriteria : CriteriaBase
{
    public int LastLogId { get; set; }

    public int BatchSize { get; set; }

    public string LastDataRow { get; set; }

    public string ServerName { get; set; }

    public int MaxLogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MatchMonitoring_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}