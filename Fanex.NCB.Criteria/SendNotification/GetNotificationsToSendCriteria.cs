namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.Data.Repository;

    public class GetNotificationsToSendCriteria : CriteriaBase
    {
        public int CreatorId { get; set; }

        public override string GetSettingKey() => "NCB_SendNotification_ScanWaitingList_Sel";

        public override bool IsValid() => true;
    }
}