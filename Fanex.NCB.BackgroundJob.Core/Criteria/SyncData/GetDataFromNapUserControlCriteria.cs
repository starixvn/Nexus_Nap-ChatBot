namespace Fanex.NCB.AutoJobs.Core.Criteria.SyncData;

using Fanex.Data.Repository;

public class GetDataFromNapUserControlCriteria : CriteriaBase
{
    public int SiteId { get; set; }

    public int? UserId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_ChatBot_GetDataBySiteId";
    }

    public override bool IsValid()
    {
        return SiteId > 0;
    }
}