namespace Fanex.NCB.Main.ViewModel.SettingConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateMappingViewModel
    {
        [Required]
        public int JobId { get; set; }

        [Required]
        public int NotificationTypeId { get; set; }

        [Required]
        public int NotificationTemplateId { get; set; }

        [Required]
        public int MsgContentsType { get; set; }

        [Required]
        public string RequiredParam { get; set; }

        [Required]
        public bool IsTest { get; set; }

        [Required]
        public int OldTemplateId { get; set; }

        [Required]
        public int OldMsgContentsType { get; set; }
    }
}