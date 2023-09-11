namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.NCB.Shared.Models;

    public class UpdateMappingCommand : BaseCommand
    {
        public int JobId { get; set; }

        public int OldNotificationTypeId { get; set; }

        public int NewNotificationTypeId { get; set; }

        public int NewNotificationTemplateId { get; set; }

        public int OldNotificationTemplateId { get; set; }

        public int NewMsgContentsType { get; set; }

        public string NewRequiredParam { get; set; }

        public bool NewIsTest { get; set; }

        public int OldMsgContentsType { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_Mapping_Upd";

        public override bool IsValid() => JobId > 0 && NewNotificationTypeId > 0 && NewNotificationTemplateId > 0;
    }
}