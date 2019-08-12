namespace Fanex.NCB.Criteria.SendNotification
{
    using Fanex.Data.Repository;

    public class GetNotificationByIdCriteria : CriteriaBase
    {
        public int NotifictaionId { get; set; }

        public override string GetSettingKey() => "NCB_SendNotification_Sel";

        public override bool IsValid() => NotifictaionId > 0;
    }
}