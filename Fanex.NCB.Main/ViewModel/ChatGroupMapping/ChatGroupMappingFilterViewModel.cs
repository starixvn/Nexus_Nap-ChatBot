namespace Fanex.NCB.Main.ViewModel.ChatGroupMapping
{
    using System.Collections.Generic;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.Setting;

    public class ChatGroupMappingFilterViewModel
    {
        public string ChannelIds { get; set; }
        public string BotIds { get; set; }
        public IEnumerable<MultiSelectViewModel> Channels { get; set; }
        public IEnumerable<MultiSelectViewModel> Bots { get; set; }
        public IEnumerable<BaseModel> RecipientTypes { get; set; }
        public IEnumerable<Recipient> Recipients { get; set; }
        public IEnumerable<ChatGroupType> ChatGroupTypes { get; set; }
        public IEnumerable<BaseModel> LicenseeStatuses { get; set; }
        public IEnumerable<BaseModel> MappingStatuses { get; set; }
    }
}