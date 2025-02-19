namespace Fanex.NCB.Criteria.LocalNap.Security;

using Data.Repository;

public class UserLogOutCriteria : CriteriaBase

{
    public int OnUserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_Logout";
    }

    public override bool IsValid()
    {
        return OnUserId > 0;
    }
}