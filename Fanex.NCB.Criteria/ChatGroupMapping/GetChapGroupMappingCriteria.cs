namespace Fanex.NCB.Criteria.ChatGroupMapping
{
    using Fanex.Data.Repository;

    public class GetChapGroupMappingCriteria : CriteriaBase
    {
        public string BotIDs { get; set; }

        public string RecipientTypeIDs { get; set; }

        public string RecipientName { get; set; }

        public string MappingStatusIds { get; set; }

        public string LicenseeStatusIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public long TotalItems { get; set; }

        public override string GetSettingKey() => "NCB_ChatGroupMapping_Sel";

        public override bool IsValid() => PageIndex > 0 && PageSize > 0;
    }
}