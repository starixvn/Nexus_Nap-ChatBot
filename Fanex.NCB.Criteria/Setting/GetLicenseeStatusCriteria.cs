namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class GetLicenseeStatusCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Setting_LicenseeStatus_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}