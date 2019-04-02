using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping
{
    public class EditChatGroupMappingCriteria
    {
        public int GroupID { get; set; }
        public int RecipientTypeID { get; set; }
        public string RecipientIDs { get; set; }       
        public int CreatorID { get; set; }
        public int ChatGroupTypeId { get; set; }
        public string Remark { get; set; }             
    }
}
