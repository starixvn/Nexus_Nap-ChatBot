namespace Fanex.NCB.AutoJobs.Core.Criteria.SyncData;

public class InsertNapUserControlDataCommand : NapUserControlSyncDataBaseCommand
{
    public override string GetSettingKey()
    {
        return "NAP_UserControl_SyncData";
    }

    public override bool IsValid()
    {
        return true;
    }
}