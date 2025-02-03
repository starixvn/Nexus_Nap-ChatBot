namespace Fanex.NCB.AutoJobs.Core.Models.Common;

public class MessageSetting
{
    public string JobStatus { get; set; }

    public int LastLogId { get; set; }

    public int BatchSize { get; set; }

    public string CarbonGroups { get; set; }

    public string MsgContentsTypeLst { get; set; }

    public string LastDataRow { get; set; }

    public string ServerName { get; set; }

    public int MaxLogId { get; set; }
}