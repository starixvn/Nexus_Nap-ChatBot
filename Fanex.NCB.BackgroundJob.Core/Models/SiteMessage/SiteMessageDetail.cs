namespace Fanex.NCB.AutoJobs.Core.Models.SiteMessage;

public class SiteMessageDetail
{
    public long MessageID { get; set; }

    public int TargetID { get; set; }

    public string SiteName { get; set; }

    public int LeagueGroupID { get; set; }

    public int IsChinaView { get; set; }

    public byte SpecialPlatform { get; set; }
}