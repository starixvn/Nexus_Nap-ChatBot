namespace Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentLicensee;

using Fanex.Data.Repository;

public class GetMessageResultAmendmentLicenseeCriteria : CriteriaBase
{
    public int BatchSize { get; set; }

    public int LastMsgTargetId { get; set; }

    public string SiteAsJSON { get; set; }

    public string DepCustSAsJSON { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_ResultAmendment_Licensee_MessageSel";
    }

    public override bool IsValid()
    {
        return true;
    }
}