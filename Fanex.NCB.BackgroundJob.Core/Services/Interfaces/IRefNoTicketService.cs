namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models.SpecialDeleteTicket.TicketInfo;

    public interface IRefNoTicketService
    {
        Task<TicketInfo> GetTicketByRefNoAsync(long refno);

        Task<(long, string)> GetTransInfoAsync(long refno);
    }
}