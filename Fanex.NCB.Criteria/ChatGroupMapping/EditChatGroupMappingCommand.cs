namespace Fanex.NCB.Criteria.ChatGroupMapping
{
    using Fanex.NCB.Shared.Models;

    public class EditChatGroupMappingCommand : BaseCommand
    {
        public int GroupID { get; set; }

        public int RecipientTypeID { get; set; }

        public string RecipientIDs { get; set; }

        public int CreatorID { get; set; }

        public int ChatGroupTypeId { get; set; }

        public string Remark { get; set; }

        public override string GetSettingKey() => "NCB_ChatGroupMapping_Upd";

        public override bool IsValid() => GroupID > 0 && RecipientTypeID > 0 && CreatorID > 0 && ChatGroupTypeId > 0;
    }
}