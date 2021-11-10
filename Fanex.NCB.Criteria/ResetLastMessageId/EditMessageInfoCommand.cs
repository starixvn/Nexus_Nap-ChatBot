namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;

    public class EditMessageInfoCommand : BaseCommand
    {
        [SpParam(Ignored = true)]
        public List<CurrentMaxMessageInfo> JobInfoList { get; set; } = new List<CurrentMaxMessageInfo>();

        public string JobInfo { get; set; }

        public override string GetSettingKey() => "NCB_JobInfo_Upd";

        public override bool IsValid() => true;
    }
}
