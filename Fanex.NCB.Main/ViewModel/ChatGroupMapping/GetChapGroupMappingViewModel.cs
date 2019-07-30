namespace Fanex.NCB.Main.ViewModel.ChatGroupMapping
{
    public class GetChapGroupMappingViewModel
    {
        public string BotNames { get; set; }

        public string RecipientTypeIDs { get; set; }

        public string RecipientName { get; set; }

        public string MappingStatusIds { get; set; }

        public string LicenseeStatusIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}