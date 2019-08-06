namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetConversationGroupCriteria : CriteriaBase
    {
        public string ConversationId { get; set; }

        public string BotName { get; set; }

        public override string GetSettingKey() => "NCB_Conversation_Sel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId) && !string.IsNullOrWhiteSpace(BotName);
    }
}