namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;

    public interface ISiteDbService
    {
        Task<IEnumerable<SiteDBConfig>> GetListSiteDbAsync();

        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> GetTicketUsernamesAynnc(
            IEnumerable<AbnormalTransId> transIds, IEnumerable<AbnormalUsernames> usernames, IEnumerable<SiteDBConfig> siteDbList);

        Task<(IEnumerable<AbnormalTicketInfo>, IEnumerable<AbnormalUsernames>)> GetTicketUsernamesParalellAynnc(
            IEnumerable<AbnormalTransId> transIds, IEnumerable<AbnormalUsernames> usernames, IEnumerable<SiteDBConfig> siteDbList);
    }
}