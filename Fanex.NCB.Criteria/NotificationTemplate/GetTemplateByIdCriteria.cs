namespace Fanex.NCB.Criteria.NotificationTemplate
{
    using Fanex.Data.Repository;

    public class GetTemplateByIdCriteria : CriteriaBase
    {
        public int NotificationTemplateId { get; set; }

        public override string GetSettingKey() => "NCB_NotificationTemplate_Sel";

        public override bool IsValid() => NotificationTemplateId > 0;
    }
}