namespace Fanex.NCB.Main.ViewModel.SettingConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateJobInfoViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int BatchSize { get; set; }

        [Required]
        public string TestingGroup { get; set; }
    }
}