namespace Fanex.NCB.Models.SettingConfiguration
{
    public class CreateMapping
    {
        public int JobId { get; set; }

        public bool IsTest { get; set; }

        public int MsgContentsType { get; set; }

        public int NotificationTemplateID { get; set; }

        public int NotificationTypeID { get; set; }

        public string RequiredParams { get; set; }

        public int CreatorId { get; set; }
    }
}