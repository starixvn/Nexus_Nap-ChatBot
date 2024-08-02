namespace Fanex.NCB.AutoJobs.Core.Criteria.SyncData
{
    public class UpdateNapDataByUserIdCommand : NapUserControlSyncDataBaseCommand
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "NAP_UserControl_SyncData_ByUserId";

        public override bool IsValid() => UserId > 0;
    }
}