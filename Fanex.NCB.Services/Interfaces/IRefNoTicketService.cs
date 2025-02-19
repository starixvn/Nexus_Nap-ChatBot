namespace Fanex.NCB.Services.Interfaces;

using System.Threading.Tasks;
using Fanex.NCB.Shared.Models.SpecialDeleteTicket.TicketInfo;

public interface IRefNoTicketService
{
    Task<TicketInfo> GetTicketByRefNoAsync(long refno);

    Task<(long, string)> GetTransInfoAsync(long refno);
}