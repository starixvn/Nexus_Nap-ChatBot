namespace Fanex.NCB.Criteria.Common;

using Data.Repository;

public class GetUsersCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "NCB_Common_User_Sel";
    }

    public override bool IsValid()
    {
        return true;
    }
}