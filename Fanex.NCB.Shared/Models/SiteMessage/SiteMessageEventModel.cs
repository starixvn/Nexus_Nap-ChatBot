namespace Fanex.NCB.Shared.Models.SiteMessage
{
    using System;

    public class SiteMessageEventModel
    {
        public int MatchId { get; set; }

        public string SportType { get; set; }

        public string SportNameEN { get; set; }

        public string SportNameCS { get; set; }

        public DateTime? KickOffTime { get; set; }

        public bool IsTest { get; set; }

        public string LeagueNameEN { get; set; }

        public string LeagueNameCS { get; set; }

        public string HomeNameEN { get; set; }

        public string HomeNameCS { get; set; }

        public string AwayNameEN { get; set; }

        public string AwayNameCS { get; set; }
    }
}