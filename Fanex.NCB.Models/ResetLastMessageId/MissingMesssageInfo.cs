namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System;
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MissingMesssageInfo
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("MsgTargetId")]
        public int MsgTargetId { get; set; }

        [XmlAttribute("MessageIds")]
        public string MessageIds { get; set; }

        [XmlAttribute("Content_en")]
        public string Content_en { get; set; }

        [XmlAttribute("Content")]
        public string Content { get; set; }

        [XmlAttribute("PostTime")]
        public DateTime PostTime { get; set; }

        [XmlAttribute("JobIdentify")]
        public int JobIdentify { get; set; }

        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("SettingName")]
        public string SettingName { get; set; }
    }
}