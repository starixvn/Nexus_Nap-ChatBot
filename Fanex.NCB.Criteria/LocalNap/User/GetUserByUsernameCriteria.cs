namespace Fanex.NCB.Criteria.LocalNap.User
{
    using Fanex.Data.Repository;

    public class GetUserByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "NAP_User_GetByUsername";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
    }
}