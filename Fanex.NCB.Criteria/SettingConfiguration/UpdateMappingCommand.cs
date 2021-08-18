namespace Fanex.NCB.Criteria.SettingConfiguration
{
    using Fanex.NCB.Shared.Models;

    public class UpdateMappingCommand : BaseCommand
    {
        public int JobId { get; set; }

        public int OldNotificationTypeId { get; set; }

        public int NotificationTypeId { get; set; }

        public int NotificationTemplateId { get; set; }

        public int OldNotificationTemplateId { get; set; }

        public int MsgContentsType { get; set; }

        public string RequiredParam { get; set; }

        public bool IsTest { get; set; }

        public int OldMsgContentsType { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "NCB_SettingConfiguration_Mapping_Upd";

        public override bool IsValid() => JobId > 0 && NotificationTypeId > 0 && NotificationTemplateId > 0 && MsgContentsType > 0;
    }
}