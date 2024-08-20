namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class CurrentMaxMessageInfo
    {
        [XmlAttribute("JobIdentify")]
        public int JobIdentify { get; set; }

        [XmlAttribute("ReferenceValue")]
        public string ReferenceValue { get; set; }

        [XmlAttribute("SettingName")]
        public string SettingName { get; set; }

        [XmlIgnore]
        public string MaxReferenceId { get; set; }
    }
}