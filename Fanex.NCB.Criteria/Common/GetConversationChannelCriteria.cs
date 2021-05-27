namespace Fanex.NCB.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetConversationChannelCriteria : CriteriaBase
    {
        public byte? Id { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "NCB_Channel_Sel";

        public override bool IsValid() => true;
    }
}