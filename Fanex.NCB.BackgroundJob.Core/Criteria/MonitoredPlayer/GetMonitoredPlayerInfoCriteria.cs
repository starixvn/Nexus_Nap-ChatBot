namespace Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;

using Fanex.Data.Repository;

public class GetMonitoredPlayerInfoCriteria : CriteriaBase
{
    public int LastLogId { get; set; }

    public int BatchSize { get; set; }

    public string LastDataRow { get; set; }

    public string ServerName { get; set; }

    public int MaxLogId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MonitoredPlayer_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}