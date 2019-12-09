namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISpecialDeleteTicketService
    {
        Task PullTicketAndCreateNotification();
    }
}