namespace Fanex.NCB.Shared.Models.SiteMessage
{
    using System;

    public class SiteMessageMatchInfoModel
    {
        public int MatchId { get; set; }

        public int SportType { get; set; }

        public string SportNameEN { get; set; } = string.Empty;

        public string SportNameCS { get; set; } = string.Empty;

        public DateTime? KickOffTime { get; set; }

        public bool IsTest { get; set; }

        public string LeagueNameEN { get; set; } = string.Empty;

        public string LeagueNameCS { get; set; } = string.Empty;

        public string HomeNameEN { get; set; } = string.Empty;

        public string HomeNameCS { get; set; } = string.Empty;

        public string AwayNameEN { get; set; } = string.Empty;

        public string AwayNameCS { get; set; } = string.Empty;
    }
}