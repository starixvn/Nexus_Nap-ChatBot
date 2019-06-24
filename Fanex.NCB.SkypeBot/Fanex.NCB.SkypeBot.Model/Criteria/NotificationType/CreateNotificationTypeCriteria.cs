namespace Fanex.NCB.SkypeBot.Model.Criteria.NotificationType
{
    public class CreateNotificationTypeCriteria
    {
        public int NotificationTypeId { get; set; }
        public string NotificationTypeName { get; set; }
        public string RecipientTypeIds { get; set; }
        public int SystemId { get; set; }
    }
}