namespace Fanex.NCB.AutoJobs.Core.Models.MonitorMatch;

public class MonitorMatchInfo
{
    public int LogId { get; set; }

    public int MatchId { get; set; }

    public string HomeTeam { get; set; }

    public string AwayTeam { get; set; }

    public DateTime EventDate { get; set; }

    public short IsLive { get; set; }

    public int HomeScore { get; set; }

    public int AwayScore { get; set; }

    public string BaseCurrency { get; set; }

    public decimal NegProfitForecast { get; set; }

    public decimal HomeStakePercentage { get; set; }

    public decimal AwayStakePercentage { get; set; }

    public string LicenseeName { get; set; }

    public short IsTest { get; set; }

    public string SettingInfo { get; set; }
}
