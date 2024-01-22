namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetVoidingHistoryInternalCSDInfoCriteria : CriteriaBase
    {
        public string MessageInfo { get; set; }

        public string MessageDetail { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_InternalCSD_Info";

        public override bool IsValid() => true;
    }
}
