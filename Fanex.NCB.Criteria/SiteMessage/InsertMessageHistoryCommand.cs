namespace Fanex.NCB.Criteria.SiteMessage
{
    using Fanex.NCB.Shared.Models;

    public class InsertMessageHistoryCommand : BaseCommand
    {
        public string MessageHeader { get; set; }

        public string MessageDetail { get; set; }

        public string Environment { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MessageHistory_Ins";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageHeader)
            && !string.IsNullOrWhiteSpace(MessageDetail);
    }
}