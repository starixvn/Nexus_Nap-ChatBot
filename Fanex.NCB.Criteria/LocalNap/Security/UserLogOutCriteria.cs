namespace Fanex.NCB.Criteria.LocalNap.Security
{
    using Fanex.Data.Repository;

    public class UserLogOutCriteria : CriteriaBase

    {
        public int OnUserId { get; set; }

        public override string GetSettingKey() => "NAP_User_Logout";

        public override bool IsValid() => OnUserId > 0;
    }
}