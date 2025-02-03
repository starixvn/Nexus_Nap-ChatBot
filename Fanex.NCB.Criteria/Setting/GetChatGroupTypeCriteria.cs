namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class GetChatGroupTypeCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Setting_ChatGroupType_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}