namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class ImportAbnormalBetsMessageCriteria : CriteriaBase
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public string DuplicatedInfo { get; set; }

        public string AbnormalAccountInfo { get; set; }

        public int InMsgTargetId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "NCB_SpecialDeleteMsg_AbnormalBets_Import";

        public override bool IsValid() => true;
    }
}