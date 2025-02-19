namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class GetBotNameCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Setting_BotName_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}