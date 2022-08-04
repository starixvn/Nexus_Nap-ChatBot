namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    public class AbnormalTicketInfo
    {
        public long MessageId { get; set; }
        public string Recommend { get; set; }
        public int Bettype { get; set; }
        public int LiveIndicator { get; set; }
        public bool IsParlay { get; set; }
    }
}