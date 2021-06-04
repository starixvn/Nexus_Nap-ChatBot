namespace Fanex.NCB.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetNotificationsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "notificationTypeIDLst")]
        public string NotificationTypeIds { get; set; }

        [SpParam(SqlParamName = "recipientIDLst")]
        public string RecipientIds { get; set; }

        public int? CreatorId { get; set; }

        [SpParam(SqlParamName = "notificationStatusIDLst")]
        public string NotificationStatusIds { get; set; }

        [SpParam(SqlParamName = "ChannelIDLst")]
        public string ChannelIds { get; set; }

        public int? MessageId { get; set; }

        public DateTime ConfigSendTimeFrom { get; set; }

        public DateTime ConfigSendTimeTo { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "NCB_NotificationList_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}