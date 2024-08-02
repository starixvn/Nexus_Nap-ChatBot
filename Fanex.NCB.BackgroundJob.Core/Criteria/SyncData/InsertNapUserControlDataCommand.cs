namespace Fanex.NCB.AutoJobs.Core.Criteria.SyncData
{
    public class InsertNapUserControlDataCommand : NapUserControlSyncDataBaseCommand
    {
        public override string GetSettingKey() => "NAP_UserControl_SyncData";

        public override bool IsValid() => true;
    }
}