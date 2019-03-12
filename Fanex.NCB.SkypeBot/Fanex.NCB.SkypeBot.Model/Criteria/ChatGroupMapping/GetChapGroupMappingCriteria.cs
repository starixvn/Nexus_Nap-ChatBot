using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping
{
    public class GetChapGroupMappingCriteria
    {
        public string BotName { get; set; }
        public string RecipientTypeID { get; set; }
        public string RecipientName { get; set; }
        public string MappingStatusIds { get; set; }
    }
}
