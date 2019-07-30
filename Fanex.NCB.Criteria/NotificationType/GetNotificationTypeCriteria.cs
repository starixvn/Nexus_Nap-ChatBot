namespace Fanex.NCB.Criteria.NotificationType
{
    using Fanex.Data.Repository;

    public class GetNotificationTypeCriteria : CriteriaBase
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int TotalItems { get; set; }

        public override string GetSettingKey() => "NCB_NotificationType_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}