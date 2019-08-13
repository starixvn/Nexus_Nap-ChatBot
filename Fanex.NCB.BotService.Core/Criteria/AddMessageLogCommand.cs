namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.NCB.Shared.Models;

    public class AddMessageLogCommand : BaseCommand
    {
        public string BotConversationId { get; set; }

        public string ConversationId { get; set; }

        public string MessageContent { get; set; }

        public bool IsSuccessfull { get; set; }

        public string ErrorMessage { get; set; }

        public override string GetSettingKey() => "NCB_Conversation_AddMessageLog";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
            && !string.IsNullOrWhiteSpace(ConversationId);
    }
}