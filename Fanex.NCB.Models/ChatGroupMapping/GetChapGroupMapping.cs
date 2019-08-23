namespace Fanex.NCB.Models.ChatGroupMapping
{
    public class GetChapGroupMapping
    {
        public string BotIDs { get; set; }

        public string RecipientTypeIDs { get; set; }

        public string RecipientName { get; set; }

        public string MappingStatusIds { get; set; }

        public string GroupName { get; set; }

        public string LicenseeStatusIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}