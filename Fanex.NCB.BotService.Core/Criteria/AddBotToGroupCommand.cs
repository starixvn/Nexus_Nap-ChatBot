namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.NCB.Shared.Models;

    public class AddBotToGroupCommand : BaseCommand
    {
        public string BotConversationId { get; set; }
        public string BotName { get; set; }
        public string ConversationId { get; set; }
        public byte ChannelId { get; set; }
        public string ServiceUrl { get; set; }
        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "NCB_Conversation_AddBotToGroup";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
            && !string.IsNullOrWhiteSpace(ConversationId)
            && ChannelId > 0;
    }
}