namespace Fanex.NCB.Criteria.SiteMessage
{
    using Fanex.NCB.Shared.Models;

    public class InsertMessageHistoryCommand : BaseCommand
    {
        public string MessageIDLst { get; set; }

        public string Environment { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MsgChatBot_Ins";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MessageIDLst);
    }
}