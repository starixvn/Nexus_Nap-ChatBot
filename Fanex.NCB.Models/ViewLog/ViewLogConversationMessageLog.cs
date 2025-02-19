namespace Fanex.NCB.Models.ViewLog;

using System;

public class ViewLogConversationMessageLog
{
    public int NotificationID { get; set; }

    public string ToConversationID { get; set; }

    public string BotName { get; set; }

    public int ConversationGroupID { get; set; }

    public string ConversationGroupName { get; set; }

    public DateTime SentTime { get; set; }

    public int IsSuccessful { get; set; }

    public string ErrorMessage { get; set; }

    public string Content { get; set; }
}