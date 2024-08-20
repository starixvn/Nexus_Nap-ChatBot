namespace Fanex.NCB.AutoJobs.Core.Models.Common
{
    using System;

    public class ServiceSetting
    {
        public string JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int LastMsgTargetId { get; set; }

        public string NotificationTemplate { get; set; }

        public string TestingGroup { get; set; }

        public int LastVoidingHistoryCollectionId { set; get; }

        public DateTime LastModifiedTime { get; set; }
    }
}