namespace Fanex.NCB.Criteria.NotificationTemplate;

using Data.Repository;

public class GetTemplateByIdCriteria : CriteriaBase
{
    public int NotificationTemplateId { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationTemplate_Sel";
    }

    public override bool IsValid()
    {
        return NotificationTemplateId > 0;
    }
}