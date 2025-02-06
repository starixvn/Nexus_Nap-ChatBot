namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.Data;
    using Fanex.NCB.Shared.Models;

    public class AddMessageLogCommand : BaseCommand
    {
        public int ReferenceId { get; set; }
        public string FromConversationId { get; set; }
        public string ToConversationId { get; set; }
        public string Content { get; set; }
        public bool IsSuccessfull { get; set; }

        [SpParam(AllowNull = true)]
        public string ErrorMessage { get; set; }

        [SpParam(AllowNull = true)]
        public string FileName { get; set; } = string.Empty;

        public override string GetSettingKey() => "NCB_Conversation_AddMessageLog";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(FromConversationId)
            && !string.IsNullOrWhiteSpace(ToConversationId);
    }
}