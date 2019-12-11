namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    public class SystemSetting
    {
        public int JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int MaxMessageID { get; set; }

        public string MsgContentsTypeLst { get; set; }
    }
}