namespace Fanex.NCB.Main.ViewModel.SettingConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class CreateMappingViewModel
    {
        [Required] public int JobId { get; set; }

        [Required] public int NotificationTypeId { get; set; }

        [Required] public int NotificationTemplateId { get; set; }

        [Required] public int MsgContentsType { get; set; }

        [Required]
        [RegularExpression(@"(\s*{[-_\w\s]+}\s*,\s*)*{[-_\w\s]+}\s*", ErrorMessage = "Invalid Required Params format")]
        public string RequiredParam { get; set; }

        [Required] public bool IsTest { get; set; }
    }
}