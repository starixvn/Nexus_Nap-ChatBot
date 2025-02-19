namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

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