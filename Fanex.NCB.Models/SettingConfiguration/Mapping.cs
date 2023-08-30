namespace Fanex.NCB.Models.SettingConfiguration
{
    public class Mapping
    {
        public bool IsTest { get; set; }

        public int MsgContentsType { get; set; }

        public string MsgContentsName { get; set; }

        public int NotificationTemplateID { get; set; }

        public string NotificationTemplateName { get; set; }

        public int NotificationTypeID { get; set; }

        public string NotificationTypeName { get; set; }

        public string SystemName { get; set; }

        public string RecipientTypeNames { get; set; }

        public string RequiredParams { get; set; }
        public bool IsProtected { get; set; }
    }
}