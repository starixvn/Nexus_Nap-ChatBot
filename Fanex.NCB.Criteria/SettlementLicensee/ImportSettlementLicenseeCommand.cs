namespace Fanex.NCB.Criteria.SettlementLicensee
{
    using Fanex.NCB.Shared.Models;

    public class ImportSettlementLicenseeCommand : BaseCommand
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public string MessageTarget { get; set; }

        public override string GetSettingKey() => "NCB_Settlement_Licensee_Import";

        public override bool IsValid() => true;
    }
}
