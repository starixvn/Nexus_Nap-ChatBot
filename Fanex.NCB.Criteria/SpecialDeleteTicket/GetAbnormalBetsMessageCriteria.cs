namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetAbnormalBetsMessageCriteria : CriteriaBase
    {
        public string MessageIDLst { get; set; }

        public int BatchSize { get; set; }

        public int LastMsgTargetId { get; set; }

        public int MaxMsgtargetId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDeleteMsg_AbnormalBets_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageIDLst);
    }
}