namespace Fanex.NCB.Criteria.LocalNap.Security;

using Data.Repository;

public class UserReCheckInUserCriteria : CriteriaBase
{
    public int OnUserId { get; set; }
    public bool IsKickedOut { get; set; }
    public bool IsPermissionChanged { get; set; }

    public override bool IsValid()
    {
        return OnUserId > 0;
    }

    public override string GetSettingKey()
    {
        return "NAP_User_ReCheckIn";
    }
}