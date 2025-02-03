namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket;

using System;

public class SystemSetting
{
    public int JobStatus { get; set; }

    public int BatchSize { get; set; }

    public int MaxMessageID { get; set; }

    public int LastVoidingHistoryCollectionId { get; set; }

    public string MsgContentsTypeLst { get; set; }

    public DateTime? LastPostTime { get; set; }
}