namespace Fanex.NCB.Criteria.LocalNap.User
{
    using Data.Repository;

    public class GetUserFunctionPolicyCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public string FunctionKey { get; set; }

        public override string GetSettingKey() => "NAP_User_GetUserSiteFunctionKeyPolicy";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && !string.IsNullOrWhiteSpace(FunctionKey);
    }
}