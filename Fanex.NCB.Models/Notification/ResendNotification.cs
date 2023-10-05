namespace Fanex.NCB.Models.Notification
{
    using System.Collections.Generic;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;

    public class ResendNotification : ConversationChannel
    {
        public int NotificationId { get; set; }

        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public IEnumerable<Recipient> Recipients { get; set; }

        public IEnumerable<Platform> Platforms { get; set; }

        public IEnumerable<IntegrationMethodInfo> IntegrationMethods { get; set; }
    }
}