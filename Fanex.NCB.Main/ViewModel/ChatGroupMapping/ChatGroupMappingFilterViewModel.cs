namespace Fanex.NCB.Main.ViewModel.ChatGroupMapping
{
    using System.Collections.Generic;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.Setting;

    public class ChatGroupMappingFilterViewModel
    {
        public IEnumerable<BotName> BotNames { get; set; }

        public IEnumerable<BaseModel> RecipientTypes { get; set; }

        public IEnumerable<BaseModel> FilterRecipientTypes { get; set; }

        public IEnumerable<Recipient> Recipients { get; set; }

        public IEnumerable<ChatGroupType> ChatGroupTypes { get; set; }

        public IEnumerable<BaseModel> LicenseeStatuses { get; set; }

        public IEnumerable<BaseModel> MappingStatuses { get; set; }
    }
}