using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping
{
    public class GetChapGroupMappingCriteria
    {
        public string BotNames { get; set; }
        public string RecipientTypeIDs { get; set; }
        public string RecipientName { get; set; }
        public string MappingStatusIds { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
