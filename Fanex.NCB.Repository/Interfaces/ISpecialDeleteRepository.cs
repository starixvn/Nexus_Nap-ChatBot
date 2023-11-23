namespace Fanex.NCB.Repository.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;

    public interface ISpecialDeleteRepository
    {
        Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>, IEnumerable<AbnormalAccountInfo>, IEnumerable<DuplicateTicketDetail>)> FetchVoidingHistoryInfo(ICriteria criteria);

        Task<(IEnumerable<CollectionTransId>, IEnumerable<CollectionUsername>, IEnumerable<CollectionDuplicatedTickets>, IEnumerable<AbnormalMessage>)> FetchVoidingHistoryMessage(ICriteria criteria);
    }
}
