namespace Fanex.NCB.AutoJobs.Core.Models.ResultAmendment
{
    public class MessageAmendmentCsd
    {
        public int NotificationId { get; set; }

        public int MessageId { get; set; }

        public int MsgContentsType { get; set; }

        public string SiteNameGroup { get; set; }

        public string Content { get; set; }

        public string Remark { get; set; }

        public string FileName { get; set; }

        public byte IsTest { get; set; }
    }
}