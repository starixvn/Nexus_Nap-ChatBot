namespace Fanex.NCB.Models.SettingConfiguration
{
    public class JobInfo
    {
        public string Status { get; set; }

        public int BatchSize { get; set; }

        public int PreviousMaxMessageId { get; set; }

        public int TestingGroup { get; set; }
    }
}