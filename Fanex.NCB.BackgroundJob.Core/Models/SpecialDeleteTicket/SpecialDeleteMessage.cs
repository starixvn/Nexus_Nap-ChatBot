namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket
{
    public class SpecialDeleteMessage
    {
        public long MessageId { get; set; }

        public string Site { get; set; }

        public string ContentEN { get; set; }

        public string ContentCS { get; set; }

        public int TicketType { get; set; }
    }
}