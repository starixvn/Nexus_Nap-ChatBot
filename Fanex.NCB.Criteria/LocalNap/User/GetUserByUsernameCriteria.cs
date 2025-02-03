namespace Fanex.NCB.Criteria.LocalNap.User;

using Data.Repository;

public class GetUserByUsernameCriteria : CriteriaBase
{
    public string Username { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_User_GetByUsername";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Username);
    }
}