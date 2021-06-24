namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISendNotificationService
    {
        Task ScanAndSendNotificationAsync();

        Task SendNotificationImmediatelyAsync(int notifiactionId);
    }
}