namespace Fanex.NCB.Models.SettingConfiguration
{
    public class UpdateJobInfo
    {
        public int JobIdentify { get; set; }

        public int BatchSize { get; set; }

        public string TestingGroup { get; set; }

        public string JobStatus { get; set; }

        public int PreviousMaxMessageId { get; set; }

        public int LastMsgTargetId { get; set; }

        public int LastVoidingHistoryCollectionId { get; set; }

        public int LastLogId { get; set; }
    }
}