namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalBetsMessageCriteria : CriteriaBase
    {
        public int MessageID { get; set; }

        public int BatchSize { get; set; }

        public string MsgContentsTypeLst { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDeleteMsg_AbnormalBets_Sel";

        public override bool IsValid() => MessageID > 0 &&
                                          BatchSize > 0 &&
                                          !string.IsNullOrWhiteSpace(MsgContentsTypeLst);
    }
}