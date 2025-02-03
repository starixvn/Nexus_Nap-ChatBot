namespace Fanex.NCB.AutoJobs.Core.Criteria.SyncData;

using Fanex.Data.Repository;

public class GetDataFromOicCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NAP_Chatbot_GetDataToSync";
    }

    public override bool IsValid()
    {
        return true;
    }
}