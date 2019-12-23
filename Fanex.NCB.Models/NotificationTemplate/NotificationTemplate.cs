namespace Fanex.NCB.Models.NotificationTemplate
{
    public class NotificationTemplate
    {
        public int NotificationTemplateId { get; set; }

        public string NotificationTemplateName { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int NotificationTypeId { get; set; }

        public string NotificationTypeName { get; set; }

        public bool UnRemovable { get; set; }
    }
}