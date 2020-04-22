namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.Data.Repository;

    public class GetMsgContentsTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_SettingConfiguration_MsgContentsTypeList_Sel";

        public override bool IsValid() => true;
    }
}