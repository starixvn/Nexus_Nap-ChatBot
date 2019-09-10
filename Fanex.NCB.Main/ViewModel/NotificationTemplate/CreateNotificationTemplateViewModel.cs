namespace Fanex.NCB.Main.ViewModel.NotificationTemplate
{
    using System.ComponentModel.DataAnnotations;

    public class CreateNotificationTemplateViewModel
    {
        [Required]
        public int NotificationTypeId { get; set; }

        [Required]
        public string NotificationTemplateName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}