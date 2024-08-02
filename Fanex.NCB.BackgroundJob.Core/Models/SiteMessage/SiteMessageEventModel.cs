namespace Fanex.NCB.AutoJobs.Core.Models.SiteMessage
{
    public class SiteMessageMatchInfoModel
    {
        public int MatchID { get; set; }

        public int SportType { get; set; }

        public string SportNameEN { get; set; } = string.Empty;

        public string SportNameCS { get; set; } = string.Empty;

        public string KickOffTime { get; set; }

        public bool IsTestMatch { get; set; }

        public string LeagueNameEN { get; set; } = string.Empty;

        public string OrigLeagueNameEN { get; set; } = string.Empty;

        public string LeagueNameCS { get; set; } = string.Empty;

        public string OrigLeagueNameCS { get; set; } = string.Empty;

        public string HomeNameEN { get; set; } = string.Empty;

        public string HomeNameCS { get; set; } = string.Empty;

        public string AwayNameEN { get; set; } = string.Empty;

        public string AwayNameCS { get; set; } = string.Empty;
    }
}