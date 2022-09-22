namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageTypeInfo
    {
        [XmlAttribute("JobIdentify")]
        public int JobIdentify { get; set; }

        [XmlAttribute("MsgContentsType")]
        public int MsgContentsType { get; set; }
    }
}