namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class GetRecipientCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Setting_Recipient_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}