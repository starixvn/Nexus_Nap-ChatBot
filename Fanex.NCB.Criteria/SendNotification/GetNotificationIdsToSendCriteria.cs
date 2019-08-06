namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.Data.Repository;

    public class GetNotificationIdsToSendCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NCB_SendNotification_ScanWaitingList_Sel";

        public override bool IsValid() => true;
    }
}