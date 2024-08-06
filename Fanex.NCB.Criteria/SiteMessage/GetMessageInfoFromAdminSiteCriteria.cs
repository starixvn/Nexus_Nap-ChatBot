namespace Fanex.NCB.Criteria.SiteMessage
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMessageInfoFromAdminSiteCriteria : CriteriaBase
    {
        public int MessageID { get; set; }

        public int BatchSize { get; set; }

        public string MsgContentsTypeLst { get; set; }

        [SpParam(AllowNull = true)]
        public DateTime? LastPostTime { get; set; }

        public string Environment { get; set; } = "UAT";

        public string SiteAsJson { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SiteMesssage_Sel";

        public override bool IsValid() => MessageID > 0 &&
                                          BatchSize > 0 &&
                                          !string.IsNullOrWhiteSpace(MsgContentsTypeLst);
    }
}