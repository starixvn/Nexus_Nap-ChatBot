namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Data.Repository;

    public class GetMappingCriteria : CriteriaBase
    {
        public int JobId { get; set; }

        public int MsgContentsType { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_Mapping_Sel";

        public override bool IsValid() => true;
    }
}