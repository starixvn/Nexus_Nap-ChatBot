namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.NCB.Shared.Models;

    public class EditMessageInfoCommand : BaseCommand
    {
        public string JobInfo { get; set; }

        public string JobDetail { get; set; }

        public override string GetSettingKey() => "NCB_JobInfo_Upd";

        public override bool IsValid() => true;
    }
}
