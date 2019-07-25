namespace Fanex.NCB.Criteria.Setting
{
    using Fanex.Data.Repository;

    public class GetLicenseeStatusCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_Setting_LicenseeStatus_Sel";

        public override bool IsValid() => true;
    }
}