namespace Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;

using Fanex.Data.Repository;

public class GetJsonDataOnDbACCCriteria : CriteriaBase
{
    public string TableName { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_DataToJSON_Sel";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(TableName);
    }
}