namespace Fanex.NCB.Models.Notification
{
    using System.Collections.Generic;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;

    public class ResendNotificationRepository
    {
        public IEnumerable<ResendNotification> ResendNotifications { get; set; }

        public IEnumerable<Recipient> Recipients { get; set; }

        public IEnumerable<Platform> Platforms { get; set; }

        public IEnumerable<IntegrationMethodInfo> IntegrationMethods { get; set; }
    }
}
