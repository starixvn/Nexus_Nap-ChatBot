namespace Fanex.NCB.Models.SettingConfiguration
{
    using System;

    public class JobInfo
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public int BatchSize { get; set; }

        public int PreviousMaxMessageId { get; set; }

        public string TestingGroup { get; set; }

        public string JobStatus { get; set; }

        public string NotificationTemplate { get; set; }

        public DateTime LastPostTime { get; set; }

        public int LastVoidingHistoryCollectionId { get; set; }

        public int LastMsgTargetId { get; set; }

        public string ServerName { get; set; }

        public string LastDataRow { get; set; }
    }
}