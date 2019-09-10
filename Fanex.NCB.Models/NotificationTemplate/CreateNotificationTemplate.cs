namespace Fanex.NCB.Models.NotificationTemplate
{
    public class CreateNotificationTemplate
    {
        public int NotificationTypeId { get; set; }

        public string NotificationTemplateName { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int CreatorId { get; set; }
    }
}