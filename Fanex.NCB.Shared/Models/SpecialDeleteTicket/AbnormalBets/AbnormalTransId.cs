namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    public class AbnormalTransId
    {
        public long MessageId { get; set; }
        public long VoidingHistoryId { get; set; }
        public string TransId { get; set; }
        public int FilterBy { get; set; }
        public int IssueTypeId { get; set; }
    }
}