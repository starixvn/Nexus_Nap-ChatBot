namespace Fanex.NCB.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetDataFromOICCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_Chatbot_GetDataToSync";

        public override bool IsValid() => true;
    }
}