namespace Fanex.NCB.Shared.Models.SiteMessage
{
    public class SiteMessageModel : SiteMessageMatchInfoModel
    {
        public long MessageID { get; set; }

        public int MsgContentsType { get; set; }

        public string Content_en { get; set; }

        public string Content_cs { get; set; }
    }
}