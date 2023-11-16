namespace Fanex.NCB.Criteria.LocalNap.Security
{
    using System;
    using Fanex.Data.Repository;

    public class UserSsoGetTimeUserCriteria : CriteriaBase
    {
        public DateTimeOffset Time { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "NAP_User_SSOGetTime";
    }
}