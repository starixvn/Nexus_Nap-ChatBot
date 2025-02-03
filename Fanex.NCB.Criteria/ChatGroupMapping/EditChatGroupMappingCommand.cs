namespace Fanex.NCB.Criteria.ChatGroupMapping;

using Data;
using Fanex.NCB.Shared.Models;

public class EditChatGroupMappingCommand : BaseCommand
{
    public int GroupID { get; set; }

    public int RecipientTypeID { get; set; }

    [SpParam(AllowNull = true)] public string RecipientIDs { get; set; }

    public int CreatorID { get; set; }

    public int ChatGroupTypeId { get; set; }

    [SpParam(AllowNull = true)] public string Remark { get; set; }

    public override string GetSettingKey()
    {
        return "NCB_ChatGroupMapping_Upd";
    }

    public override bool IsValid()
    {
        return GroupID > 0 && RecipientTypeID > 0 && CreatorID > 0 && ChatGroupTypeId > 0;
    }
}