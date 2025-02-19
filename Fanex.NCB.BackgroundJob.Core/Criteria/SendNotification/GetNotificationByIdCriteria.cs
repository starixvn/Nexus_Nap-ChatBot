namespace Fanex.NCB.AutoJobs.Core.Criteria.SendNotification;

using Fanex.Data.Repository;

public class GetNotificationByIdCriteria : CriteriaBase
{
    public int NotificationId { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_SendNotification_GetImmedNotifUpdStatus";
    }

    public override bool IsValid()
    {
        return NotificationId > 0;
    }
}