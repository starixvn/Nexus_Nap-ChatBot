namespace Fanex.NCB.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetDataFromOICCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NapChatBot_GetDataFromOIC";

        public override bool IsValid() => true;
    }
}