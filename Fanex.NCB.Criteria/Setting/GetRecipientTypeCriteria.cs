namespace Fanex.NCB.Criteria.Setting;

using Data.Repository;

public class GetRecipientTypeCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_RecipientType_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}