namespace Fanex.NCB.AutoJobs.Core.Criteria.Common
{
    public class DeleteOldConversationMessageLogCommand : BaseCommand
    {
        public string CurrentDate { get; set; }

        public override string GetSettingKey() => "NCB_ViewLog_DeleteOldConversationMessageLog_Import";

        public override bool IsValid() => CurrentDate.Length > 0;
    }
}