namespace Fanex.NCB.Core.Models;

public class SignalRSettings
{
    public string HubEndpoint { get; set; }

    public string UpdateNotificationStatus { get; set; }

    public string UpdateNewNotification { get; set; }

    public string UpdateErrorNotification { get; set; }
}