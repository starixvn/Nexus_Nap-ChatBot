namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;

    public interface ISendNotificationService
    {
        Task SendMessageToRecipients();

        Task<CommandResult> UpdateNotificationSendingStatus(int notificationId, int creatorId);
    }
}