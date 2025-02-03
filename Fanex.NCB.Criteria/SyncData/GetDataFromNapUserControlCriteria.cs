namespace Fanex.NCB.Criteria.SyncData;

using Data;
using Data.Repository;

public class GetDataFromNapUserControlCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    [SpParam(AllowNull = true)] public int? UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_ChatBot_GetDataBySiteId";
    }

    public override bool IsValid()
    {
        return SiteId > 0;
    }
}