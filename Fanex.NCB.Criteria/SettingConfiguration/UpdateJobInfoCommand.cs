namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.NCB.Shared.Models;

    public class UpdateJobInfoCommand : BaseCommand
    {
        public int Id { get; set; }

        public int BatchSize { get; set; }

        public int TestingGroup { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_JobInfo_Upd";

        public override bool IsValid() => Id > 0 && BatchSize > 0 && TestingGroup > 0 && ModifiedBy > 0;
    }
}