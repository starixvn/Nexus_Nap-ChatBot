namespace Fanex.NCB.Criteria.Common
{
    using Data.Repository;

    public class GetJsonDataOnDbACCCriteria : CriteriaBase
    {
        public string TableName { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_DataToJSON_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(TableName);
    }
}