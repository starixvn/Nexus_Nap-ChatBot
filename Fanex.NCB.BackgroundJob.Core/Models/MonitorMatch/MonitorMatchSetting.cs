namespace Fanex.NCB.AutoJobs.Core.Models.MonitorMatch;

using Fanex.NCB.AutoJobs.Core.Models.Common;

public class MonitorMatchSetting : ServiceSetting
{
    public int LastLogId { get; set; } = 0;

    public string? LastDataRow { get; set;}

    public string? ServerName { get; set; }

}
