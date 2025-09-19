namespace Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentInternalCSD;

using System;
using Fanex.Data.Repository;

public class GetMessageResultAmendmentCsdCriteria : CriteriaBase
{
    public DateTime LastModifiedTime { get; set; }

    public DateTime? NewLastModifiedTime { get; set; }

    public string Environment { get; set; } = "UAT";

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_ResultAmendment_InternalCSD_MessageSel";
    }

    public override bool IsValid()
    {
        return true;
    }
}