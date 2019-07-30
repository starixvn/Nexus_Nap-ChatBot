namespace Fanex.NCB.Main.ViewModel.ChatGroupMapping
{
    public class GetChatGroupMappingParamsViewModel
    {
        public string BotIds { get; set; }

        public string RecipientTypeIds { get; set; }

        public string RecipientName { get; set; }

        public string MappingStatusIds { get; set; }

        public string LicenseeStatusIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}