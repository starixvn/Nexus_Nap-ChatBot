namespace Fanex.NCB.Criteria.LocalNap.Security;

using System;
using Data.Repository;

public class UserSsoGetTimeUserCriteria : CriteriaBase
{
    public DateTime Time { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "NAP_User_SSOGetTime";
    }
}