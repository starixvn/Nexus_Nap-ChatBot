namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    public class AbnormalBetsMessage : Message
    {
        public string RequestIDLst { get; set; }

        public string DecidedByLst { get; set; }

        public string RequestByLst { get; set; }

        public string VoidByLst { get; set; }

        public string ReasonLst { get; set; }

        public int TotalTickets { get; set; }

        public string KickOffTime { get; set; }

        public string LeagueName { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public int? HalfTimeHomeScore { get; set; }

        public int? HalfTimeAwayScore { get; set; }

        public int? FullTimeHomeScore { get; set; }

        public int? FullTimeAwayScore { get; set; }

        public string TypeNameLst { get; set; }

        public string CreditSiteLst { get; set; }

        public string DepositSiteLst { get; set; }
    }
}