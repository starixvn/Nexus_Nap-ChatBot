namespace Fanex.NCB.Main.ViewModel.SettingConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class CreateSettingTypeViewModel
    {
        [Required]
        public int Type { get; set; }

        [Required]
        public string Name { get; set; }
    }
}