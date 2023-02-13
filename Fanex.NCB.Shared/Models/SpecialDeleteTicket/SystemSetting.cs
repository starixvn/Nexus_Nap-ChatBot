namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System;

    public class SystemSetting
    {
        public int JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int MaxMessageID { get; set; }

        public int LastLogId { get; set; }

        public string MsgContentsTypeLst { get; set; }

        public DateTime? LastPostTime { get; set; }
    }
}