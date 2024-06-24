namespace Fanex.NCB.Criteria.ResultAmendmentInternalCSD
{
    using System;
    using Fanex.NCB.Shared.Models;

    public class ImportResultAmendmentCSDCommand : BaseCommand
    {
        public string Message {  get; set; }

        public DateTime? LastModifiedTime { get; set; }

        public override string GetSettingKey() => "NCB_ResultAmendment_InternalCSD_Import";

        public override bool IsValid() => true;
    }
}
