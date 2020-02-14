namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System;

    public class AbnormalBetMessage
    {
        public int MessageId { get; set; }

        public string RequestId { get; set; }

        public string DecidedBy { get; set; }

        public string RequestBy { get; set; }

        public string VoidBy { get; set; }

        public string Reason { get; set; }

        public DateTime KickOffTime { get; set; }

        public string LeagueName { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public int HalfTimeHomeScore { get; set; }

        public int HalfTimeAwayScore { get; set; }

        public int FullTimeHomeScore { get; set; }

        public int FullTimeAwayScore { get; set; }

        public string TypeNameLst { get; set; }

        public string Site { get; set; }

        public bool IsDeposit { get; set; }

        public string Content_en { get; set; }

        public string Content_cs { get; set; }
    }
}