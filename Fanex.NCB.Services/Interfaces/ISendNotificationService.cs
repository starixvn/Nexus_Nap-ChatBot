namespace Fanex.NCB.Services.Interfaces;

using System.Threading.Tasks;

public interface ISendNotificationService
{
    Task SendNotificationImmediatelyAsync(int notifiactionId);
}