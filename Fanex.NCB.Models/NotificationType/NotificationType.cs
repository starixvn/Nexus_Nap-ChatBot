namespace Fanex.NCB.Models.NotificationType
{
    using System.Collections.Generic;
    using Fanex.NCB.Core.Models;

    public class NotificationType : BaseModel
    {
        public IEnumerable<NotificationRecipientType> RecipientTypes { get; set; }

        public IEnumerable<NotificationChatGroupType> ChatGroupTypes { get; set; }

        public int SystemId { get; set; }

        public string SystemName { get; set; }
    }
}