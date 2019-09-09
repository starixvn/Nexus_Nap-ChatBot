namespace Fanex.NCB.Criteria.NotificationTemplate
{
    using Fanex.Data.Repository;

    public class GetTemplatesCriteria : CriteriaBase
    {
        public int? NotificationTypeId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "NCB_Template_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}