namespace Fanex.NCB.BotService.Core.Criteria
{
    public class AddBotToGroupCommand : BaseCommand
    {
        public string BotConversationId { get; set; }

        public string BotName { get; set; }

        public string ConversatioId { get; set; }

        public string GroupName { get; set; }

        public string ChannelId { get; set; }

        public string ServiceUrl { get; set; }

        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "NCB_Conversation_AddBotToGroup";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
            && !string.IsNullOrWhiteSpace(ConversatioId);
    }
}