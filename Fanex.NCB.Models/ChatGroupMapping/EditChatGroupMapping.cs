namespace Fanex.NCB.Models.ChatGroupMapping;

public class EditChatGroupMapping
{
    public int GroupID { get; set; }

    public int RecipientTypeID { get; set; }

    public string RecipientIDs { get; set; }

    public int CreatorID { get; set; }

    public int ChatGroupTypeId { get; set; }

    public string Remark { get; set; }
}