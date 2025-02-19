namespace Fanex.NCB.Criteria.LocalNap.User;

using Data;
using Data.Repository;

public class GetSiteLevelCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public string Environment { get; set; }

    [SpParam(AllowNull = true)] public bool? IsActive { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Site_GetByUser";
    }

    public override bool IsValid()
    {
        return UserId > 0 && !string.IsNullOrWhiteSpace(Environment);
    }
}