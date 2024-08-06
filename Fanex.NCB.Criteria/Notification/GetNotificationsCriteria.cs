namespace Fanex.NCB.Criteria.Notification
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;

    public class GetNotificationsCriteria : CriteriaBase
    {
        public string InNotificationTypeIds { get; set; }

        public string InRecipientIds { get; set; }

        [SpParam(AllowNull = true)]
        public int? InCreatorId { get; set; }

        public string InNotificationStatusId { get; set; }

        public string InChannelIds { get; set; }

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