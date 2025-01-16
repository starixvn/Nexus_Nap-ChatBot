namespace Fanex.NCB.Criteria.Setting
{
    using Data.Repository;

    public class GetBotNameCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_BotName_Sel";

        public override bool IsValid() => true;
    }
}