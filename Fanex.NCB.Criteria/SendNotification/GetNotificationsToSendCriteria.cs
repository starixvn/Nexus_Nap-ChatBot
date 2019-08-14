namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.Data.Repository;

    public class GetNotificationsToSendCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_SendNotification_GetPlanRetryNotifUpdStatus";

        public override bool IsValid() => true;
    }
}