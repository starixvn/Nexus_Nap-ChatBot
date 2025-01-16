namespace Fanex.NCB.Criteria.Common
{
    using Data;
    using Data.Repository;

    public class GetIntegrationMethodCriteria : CriteriaBase
    {
        public int InNotificationTypeId { get; set; }

        [SpParam(AllowNull = true)]
        public uint? InIntegrationMethodId { get; set; }

        [SpParam(AllowNull = true)]
        public string InIntegrationMethodName { get; set; }

        public override string GetSettingKey() => "NCB_IntegrationMethod_Sel";

        public override bool IsValid() => InNotificationTypeId > 0;
    }
}