namespace Fanex.NCB.Criteria.NotificationTemplate
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTemplatesCriteria : CriteriaBase
    {
        [SpParam(AllowNull = true)]
        public int? NotificationTypeId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "NCB_NotificationTemplateList_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}