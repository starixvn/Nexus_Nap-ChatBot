namespace Fanex.NCB.BotService.Core.Criteria
{
    public class UpdateGroupTopicCommand : BaseCommand
    {
        public string ConversationId { get; set; }

        public string GroupName { get; set; }

        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "NCB_Conversation_UpdateGroupTopic";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId)
            && !string.IsNullOrWhiteSpace(GroupName);
    }
}