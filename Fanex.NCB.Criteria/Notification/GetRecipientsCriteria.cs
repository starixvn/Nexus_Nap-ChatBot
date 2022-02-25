namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class GetRecipientsCriteria : CriteriaBase
    {
        public int NotificationTypeId { get; set; }

        public int? ProductId { get; set; }

        public string ChannelIdLst { get; set; }

        public string Platforms { get; set; }

        public override string GetSettingKey() => "NCB_NewNotification_Recipient_Sel";

        public override bool IsValid() => NotificationTypeId > 0;
    }
}