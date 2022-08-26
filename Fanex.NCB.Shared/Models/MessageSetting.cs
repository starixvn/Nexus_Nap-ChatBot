namespace Fanex.NCB.Shared.Models.ProblemAccounts
{
    public class MessageSetting
    {
        public string JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int LastLogId { get; set; }

        public string CarbonGroups { get; set; }

        public string MsgContentsTypeLst { get; set; }

        public string LastDataRow { get; set; }

        public string ServiceName { get; set; }
    }
}
