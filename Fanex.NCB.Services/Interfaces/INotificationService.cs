namespace Fanex.NCB.Services.Interfaces
{
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Shared.Models;
    using System.Threading.Tasks;

    public interface INotificationService
    {
        Task<CommandResult> CreateAsync(CreateNotification model);
    }
}