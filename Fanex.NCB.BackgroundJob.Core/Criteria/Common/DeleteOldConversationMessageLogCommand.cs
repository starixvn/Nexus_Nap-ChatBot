namespace Fanex.NCB.AutoJobs.Core.Criteria.Common;

public class DeleteOldConversationMessageLogCommand : BaseCommand
{
    public string CurrentDate { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ViewLog_DeleteOldConversationMessageLog_Import";
    }

    public override bool IsValid()
    {
        return CurrentDate.Length > 0;
    }
}