namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IMonitoredPlayerService
    {
        Task SendMessageMonitoredPlayerAsync();
    }
}
