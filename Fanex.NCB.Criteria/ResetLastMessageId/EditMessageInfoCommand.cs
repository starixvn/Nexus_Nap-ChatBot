namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;

    public class EditMessageInfoCommand : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public List<CurrentMaxMessageInfo> JobInfoList { get; set; } = new List<CurrentMaxMessageInfo>();

        public string JobInfo { get; set; }

        public override string GetSettingKey() => "NCB_JobInfo_Upd";

        public override bool IsValid() => true;
    }
}
