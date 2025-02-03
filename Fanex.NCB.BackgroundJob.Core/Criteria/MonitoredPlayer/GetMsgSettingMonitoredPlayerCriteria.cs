namespace Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;

using Fanex.Data.Repository;

public class GetMsgSettingMonitoredPlayerCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_MsgSetting_MonitoredPlayer";
    }

    public override bool IsValid()
    {
        return true;
    }
}