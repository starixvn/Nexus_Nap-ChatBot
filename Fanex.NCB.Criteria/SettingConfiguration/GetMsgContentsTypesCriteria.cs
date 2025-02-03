namespace Fanex.NCB.Criteria.SettingConfiguration;

using Data.Repository;

public class GetMsgContentsTypesCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_SettingConfiguration_MsgContentsTypeList_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}