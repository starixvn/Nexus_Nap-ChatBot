namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    public class AbnormalMessageDetail : AbnormalMessageContent
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
        public string UsernameLst { get; set; }
        public int Type { get; set; }
        public bool IsTest { get; set; }
        public int MatchId { get; set; }
        public int IssueTypeId { get; set; }
        public bool IsParlay { get; set; }
        public string ContentEN { get; set; }
        public string ContentCS { get; set; }
    }
}