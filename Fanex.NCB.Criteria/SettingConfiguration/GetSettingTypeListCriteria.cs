namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.Data.Repository;

    public class GetSettingTypeListCriteria : CriteriaBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        
        public override string GetSettingKey() => "NCB_SettingConfiguration_SettingTypeList_Sel";

        public override bool IsValid() => true;
    }
}