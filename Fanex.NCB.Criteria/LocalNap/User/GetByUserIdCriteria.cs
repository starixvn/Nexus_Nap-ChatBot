namespace Fanex.NCB.Criteria.LocalNap.User;

using Data.Repository;

public class GetByUserIdCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetById";
    }

    public override bool IsValid()
    {
        return UserId > 0;
    }
}