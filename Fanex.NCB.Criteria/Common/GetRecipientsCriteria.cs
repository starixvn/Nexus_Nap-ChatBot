namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

public class GetRecipientsCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_Recipient_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}