namespace Fanex.NCB.Criteria.SiteMessage
{
    using Fanex.Data.Repository;

    public class GetMessageInfoFromAdminSite : CriteriaBase
    {
        public int MessageID { get; set; }

        public int BatchSize { get; set; }

        public string MsgContentsTypeLst { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SiteMesssage_Sel";

        public override bool IsValid() => MessageID > 0 &&
                                          BatchSize > 0 &&
                                          !string.IsNullOrWhiteSpace(MsgContentsTypeLst);
    }
}