namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Collections.Generic;

    public class VoidingHistoryInfo
    {
        public IEnumerable<AbnormalMessageDetail> AbnormalMessageDetail { get; set; }

        public IEnumerable<AbnormalMessage> AbnormalMessage { get; set; }

        public IEnumerable<DuplicateTicketDetail> DuplicateTicketDetail { get; set; }

        public IEnumerable<AbnormalAccountInfo> AbnormalAccountInfo { get; set; }
    }
}
