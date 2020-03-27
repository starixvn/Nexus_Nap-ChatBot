namespace Fanex.NCB.Criteria.SiteMessage
{
    using Fanex.Data.Repository;

    public class GetSiteMessageSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_MsgSetting_SiteMessage_Sel";

        public override bool IsValid() => true;
    }
}