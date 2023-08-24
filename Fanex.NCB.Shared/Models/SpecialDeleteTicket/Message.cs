namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System;
    using System.Xml.Serialization;

    [XmlType("r")]
    public class Message
    {
        [XmlAttribute("MessageId")]
        public int MessageID { get; set; }

        [XmlAttribute("MsgContentsType")]
        public int MsgContentsType { get; set; }

        [XmlAttribute("Content_en")]
        public string Content_en { get; set; }

        [XmlAttribute("Content_cs")]
        public string Content_cs { get; set; }

        [XmlAttribute("RequestId")]
        public int RequestID { get; set; }

        [XmlAttribute("IsNew")]
        public bool IsNew { get; set; }

        [XmlAttribute("PostTime")]
        public DateTime PostTime { get; set; }
    }
}