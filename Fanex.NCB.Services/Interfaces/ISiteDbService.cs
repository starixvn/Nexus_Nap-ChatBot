namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;

    public interface ISiteDbService
    {
        Task<IEnumerable<SiteDBConfig>> GetListSiteDbAsync();

        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> GetTicketUsernamesAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames, IEnumerable<CollectionDuplicatedTickets> duplicatedTickets);

        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>, IEnumerable<DuplicatedTicketInfo>)> GetTicketUsernamesParalellAynnc(
            IEnumerable<CollectionTransId> transIds, IEnumerable<CollectionUsername> usernames, IEnumerable<CollectionDuplicatedTickets> duplicatedTickets);
    }
}