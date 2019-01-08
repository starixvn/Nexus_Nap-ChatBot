namespace Fanex.NCB.SkypeBot.Model.DTO.NotificationType
{
    public class NotificationTypeItem
    {
        public int NotificationTypeId { get; set;}
        public string NotificationTypeName { get; set;}
        public int RecipientTypeId { get; set;}
        public string RecipientTypeName { get; set;}
        public int SystemId { get; set;}
        public string SystemName { get; set;}
    }
}