namespace Fanex.NCB.Models.SettingConfiguration
{
    public class UpdateJobInfo
    {
        public int Id { get; set; }

        public int BatchSize { get; set; }

        public string TestingGroup { get; set; }

        public int ModifiedBy { get; set; }
    }
}