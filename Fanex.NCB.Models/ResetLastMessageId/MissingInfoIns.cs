namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System.Xml.Serialization;

    public class MissingInfoIns
    {
        [XmlAttribute("JobIdentify")]
        public int JobIdentify { get; set; }

        [XmlAttribute("MessageId")]
        public string MessageId { get; set; }

        [XmlAttribute("SettingName")]
        public string SettingName { get; set; }

        [XmlAttribute("NotificationId")]
        public int NotificationId { get; set; }
    }
}
