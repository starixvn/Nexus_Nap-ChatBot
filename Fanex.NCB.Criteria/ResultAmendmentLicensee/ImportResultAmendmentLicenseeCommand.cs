namespace Fanex.NCB.Criteria.ResultAmendmentLicensee
{
    using Fanex.NCB.Shared.Models;

    public class ImportResultAmendmentLicenseeCommand : BaseCommand
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public string MessageTarget { get; set; }

        public override string GetSettingKey() => "NCB_ResultAmendment_InternalCSD_Import";

        public override bool IsValid() => true;
    }
}
