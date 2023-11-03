namespace Fanex.NCB.Shared.Models
{
    public class ServiceSetting
    {
        public string JobStatus { get; set; }

        public int BatchSize { get; set; }

        public int LastMsgTargetId { get; set; }

        public string NotificationTemplate { get; set; }

        public string TestingGroup { get; set; }
    }
}
