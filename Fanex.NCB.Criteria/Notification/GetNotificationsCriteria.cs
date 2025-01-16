namespace Fanex.NCB.Criteria.Notification
{
    using System;
    using Data;
    using Data.Repository;

    public class GetNotificationsCriteria : CriteriaBase
    {
        [SpParam(AllowNull = true)]
        public string InNotificationTypeIds { get; set; }

        [SpParam(AllowNull = true)]
        public string InRecipientIds { get; set; } = string.Empty;

        [SpParam(AllowNull = true)]
        public int? InCreatorId { get; set; }

        [SpParam(AllowNull = true)]
        public string InNotificationStatusId { get; set; }

        [SpParam(AllowNull = true)]
        public string InChannelIds { get; set; }

        [SpParam(AllowNull = true)]
        public string InProductIds { get; set; }

        [SpParam(AllowNull = true)]
        public uint? InMessageId { get; set; }

        public DateTime InConfigSendTimeFrom { get; set; }

        public DateTime InConfigSendTimeTo { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}