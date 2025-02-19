namespace Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentInternalCSD;

using System;

public class ImportResultAmendmentCSDCommand : BaseCommand
{
    public string Message { get; set; }

    public DateTime? LastModifiedTime { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ResultAmendment_InternalCSD_Import";
    }

    public override bool IsValid()
    {
        return true;
    }
}