namespace Fanex.NCB.Criteria.NotificationTemplate;

using Data;
using Data.Repository;

public class GetTemplatesCriteria : CriteriaBase
{
    [SpParam(AllowNull = true)] public int? NotificationTypeId { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NotificationTemplateList_Sel";
    }

    public override bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0;
    }
}