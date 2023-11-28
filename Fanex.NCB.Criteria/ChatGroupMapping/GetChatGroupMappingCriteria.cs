namespace Fanex.NCB.Criteria.ChatGroupMapping
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetChatGroupMappingCriteria : CriteriaBase
    {
        public string ChannelIds { get; set; }
        public string BotIds { get; set; }

        public string RecipientTypeIDs { get; set; }

        [SpParam(AllowNull = true)]
        public string RecipientName { get; set; }

        [SpParam(AllowNull = true)]
        public string GroupName { get; set; }

        public string MappingStatusIds { get; set; }

        public string LicenseeStatusIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public long TotalItems { get; set; }

        public override string GetSettingKey() => "NCB_ChatGroupMapping_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}