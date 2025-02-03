namespace Fanex.NCB.Criteria.SendNotification;

using Fanex.NCB.Shared.Models;

public class UpdateRecipientSendNotifiactionStatusCommand : BaseCommand
{
    public string SentStatus { get; set; }

    public string UpdatedNotificationIds { get; set; }

    public bool IsHaveErrNotification { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SendNotification_RecipientSentStatus_Upd";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SentStatus);
    }
}