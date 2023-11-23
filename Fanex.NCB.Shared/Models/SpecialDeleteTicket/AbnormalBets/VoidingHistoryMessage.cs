namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Collections.Generic;

    public class VoidingHistoryMessage
    {
        public IEnumerable<CollectionTransId> CollectionTransId { get; set; }

        public IEnumerable<CollectionUsername> CollectionUsername { get; set; }

        public IEnumerable<CollectionDuplicatedTickets> CollectionDuplicatedTickets { get; set; }

        public IEnumerable<AbnormalMessage> AbnormalMessage { get; set; }
    }
}
