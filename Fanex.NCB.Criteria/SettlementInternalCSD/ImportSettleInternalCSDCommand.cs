namespace Fanex.NCB.Criteria.SettlementInternalCSD
{
    using Fanex.NCB.Shared.Models;

    public class ImportSettleInternalCSDCommand : BaseCommand
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public override string GetSettingKey() => "NCB_Settlement_InternalCSD_Import";

        public override bool IsValid() => true;
    }
}
