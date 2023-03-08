namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetVoidingHIstoryCollectionIdCriteria : CriteriaBase
    {
        public int LastVoidingHistoryCollectionId { get; set; }
        public int BatchSize { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_AbnormalBets_GetMessage";

        public override bool IsValid() => BatchSize > 0;
    }
}