namespace Fanex.NCB.Models.NotificationTemplate
{
    public class EditNotificationTemplate
    {
        public int NotificationTemplateId { get; set; }

        public int NotificationTypeId { get; set; }

        public string NotificationTemplateName { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int ModifierId { get; set; }
    }
}