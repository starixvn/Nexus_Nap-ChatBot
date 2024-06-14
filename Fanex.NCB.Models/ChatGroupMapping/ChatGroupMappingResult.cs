namespace Fanex.NCB.Models.ChatGroupMapping
{
    using Fanex.NCB.Models.Common;

    public class ChatGroupMappingResult : ChannelInfo
    {
        public string BotName { get; set; }
        public int GroupID { get; set; }
        public string ConversationID { get; set; }
        public string GroupName { get; set; }
        public int ChatGroupTypeID { get; set; }
        public string ChatGroupTypeName { get; set; }
        public int RecipientTypeID { get; set; }
        public string RecipientTypeName { get; set; }
        public string RecipientIDs { get; set; }
        public string RecipientNames { get; set; }
        public string RecipientStatusNames { get; set; }
        public int MappingStatusID { get; set; }
        public string MappingStatusName { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}