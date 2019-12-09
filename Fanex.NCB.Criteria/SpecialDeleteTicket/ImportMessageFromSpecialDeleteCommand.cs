namespace Fanex.NCB.Criteria.SpecialDeleteTicket
{
    using Fanex.NCB.Shared.Models;

    public class ImportMessageFromSpecialDeleteCommand : BaseCommand
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public override string GetSettingKey() => "NCB_SpecialDeleteMsg_Import";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageHeader) &&
                    !string.IsNullOrWhiteSpace(MessageDetail);
    }
}