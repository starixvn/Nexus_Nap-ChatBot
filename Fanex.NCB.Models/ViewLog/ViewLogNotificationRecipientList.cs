namespace Fanex.NCB.Models.ViewLog;

using System;

public class ViewLogNotificationRecipientList
{
    public int NotificationID { get; set; }

    public string Name { get; set; }

    public string ChatGroupTypeName { get; set; }

    public string RecipientName { get; set; }

    public DateTime SentTime { get; set; }

    public string Status { get; set; }

    public byte RetryTime { get; set; }

    public string ErrMsg { get; set; }
}