namespace Fanex.NCB.Criteria.ChatGroupMapping
{
    using Fanex.NCB.Shared.Models;

    public class UpdateActiveConverstionGroupCommand : BaseCommand
    {
        public int GroupID { get; set; }

        public int UserID { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "NCB_ChatGroupMapping_ActiveGroup_Upd";

        public override bool IsValid() => GroupID > 0;
    }
}