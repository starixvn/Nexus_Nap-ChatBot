namespace Fanex.NCB.AutoJobs.Core.Criteria.MonitorMatch;

using Fanex.Data.Repository;

public class ImportMsgFromMonitorMatchCriteria : CriteriaBase
{
    public string Message { get; set; }

    public string ServerName { get; set; }

    public string LastDataRow { get; set; }

    public int PLastLogId { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_MatchMonitoring_Import";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(ServerName) && !string.IsNullOrEmpty(LastDataRow);
    }
}