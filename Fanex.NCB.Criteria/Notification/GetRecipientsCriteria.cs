namespace Fanex.NCB.Criteria.Notification;

using Data;
using Data.Repository;

public class GetRecipientsCriteria : CriteriaBase
{
    public int InNotificationTypeId { get; set; }

    [SpParam(AllowNull = true)] public int? InProductId { get; set; }

    [SpParam(AllowNull = true)] public string InIntegrationMethodIds { get; set; }

    [SpParam(AllowNull = true)] public string InChannelIds { get; set; }

    [SpParam(AllowNull = true)] public string InPlatformIds { get; set; }

    [SpParam(AllowNull = true)] public string InRecipientIds { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_NewNotification_Recipient_Sel";
    }

    public override bool IsValid()
    {
        return InNotificationTypeId > 0;
    }
}