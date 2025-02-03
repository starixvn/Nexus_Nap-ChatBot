namespace Fanex.NCB.Main.ViewModel.SettingConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateJobInfoViewModel
    {
        [Required] public int Id { get; set; }

        [Required] public int BatchSize { get; set; }

        [Required] public string TestingGroup { get; set; }

        [Required] public string JobStatus { get; set; }

        public int PreviousMaxMessageId { get; set; }

        public int LastMsgTargetId { get; set; }

        public int LastVoidingHistoryCollectionId { get; set; }

        public int LastLogId { get; set; }

        public string LastModifiedTime { get; set; }
    }
}