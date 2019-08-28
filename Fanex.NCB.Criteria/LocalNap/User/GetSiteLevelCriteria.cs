namespace Fanex.NCB.Criteria.LocalNap.User
{
    using Fanex.Data.Repository;

    public class GetSiteLevelCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Environment { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "NAP_Site_GetByUser";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(Environment);
    }
}