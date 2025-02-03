namespace Fanex.NCB.Criteria.Notification;

using System;
using Data;
using Fanex.NCB.Shared.Models;

public class CreateNewNotificationCommand : BaseCommand
{
    public int InNotificationTypeId { get; set; }

    [SpParam(AllowNull = true)] public int? InProductId { get; set; }

    public string InRecipientIds { get; set; }

    public string InTitle { get; set; }

    public string InContentLst { get; set; }

    public DateTime InConfigSendTime { get; set; }

    public bool InSendImmediately { get; set; }

    public int InCreatorId { get; set; }

    public string NotificationIDLst { get; set; }

    public string InPlatformIds { get; set; }

    public string InIntegrationMethodIds { get; set; }

        [SpParam(AllowNull = true)]
        public string InFileName { get; set; }

    }

    public override bool IsValid()
    {
        var isValidNotiInfo = InNotificationTypeId > 0 && InCreatorId > 0;
        return isValidNotiInfo && !string.IsNullOrWhiteSpace(InRecipientIds) && !string.IsNullOrWhiteSpace(InTitle)
               && !string.IsNullOrWhiteSpace(InContentLst);
    }
}