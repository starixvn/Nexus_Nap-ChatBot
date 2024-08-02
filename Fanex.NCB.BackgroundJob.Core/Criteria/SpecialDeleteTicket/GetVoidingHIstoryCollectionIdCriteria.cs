namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetVoidingHistoryCollectionIdCriteria : CriteriaBase
    {
        public int LastVoidingHistoryCollectionId { get; set; }
        public int BatchSize { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_SpecialDelete_InternalCSD_GetCollection";

        public override bool IsValid() => BatchSize > 0;
    }
}