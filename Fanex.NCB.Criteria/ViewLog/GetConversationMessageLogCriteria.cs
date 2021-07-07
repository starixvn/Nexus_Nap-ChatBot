namespace Fanex.NCB.Criteria.ViewLog
{
    using Fanex.Data.Repository;

    public class GetConversationMessageLogCriteria : CriteriaBase
    {
        public int? NotificationId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "NCB_ViewLog_ConversationMessageLog_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}
