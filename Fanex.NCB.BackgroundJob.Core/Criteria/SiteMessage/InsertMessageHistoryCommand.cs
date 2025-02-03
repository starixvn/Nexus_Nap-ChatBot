namespace Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;

public class InsertMessageHistoryCommand : BaseCommand
{
    public int JobIdentify { get; set; }

    public string MessageIDLst { get; set; }

    public string Environment { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Chatbot_MsgChatBot_Ins";
    }

    public override bool IsValid()
    {
        return JobIdentify > 0 && !string.IsNullOrWhiteSpace(MessageIDLst);
    }
}