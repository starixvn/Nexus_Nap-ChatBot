namespace Fanex.NCB.AutoJobs.Core.Models.MonitoredPlayer;

public class MonitoredPlayerInfo
{
    public int LogId { get; set; }

    public int CustId { get; set; }

    public string Username { get; set; }

    public string Username2 { get; set; }

    public string SiteName { get; set; }

    public decimal AccumWL { get; set; }

    public decimal WinningThreshold { get; set; }

    public int IsTest { get; set; }

    public string CurrencyName { get; set; }
}