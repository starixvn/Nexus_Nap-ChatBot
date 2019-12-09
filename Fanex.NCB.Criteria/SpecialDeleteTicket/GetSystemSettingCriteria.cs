namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.Data.Repository;

    public class GetSystemSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_SpecialDelete_Sel";

        public override bool IsValid() => true;
    }
}