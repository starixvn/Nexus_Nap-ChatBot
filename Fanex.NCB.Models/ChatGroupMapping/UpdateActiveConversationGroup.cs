namespace Fanex.NCB.Models.ChatGroupMapping
{
    public class UpdateActiveConversationGroup
    {
        public int GroupID { get; set; }

        public int UserID { get; set; }

        public bool IsActive { get; set; }
    }
}