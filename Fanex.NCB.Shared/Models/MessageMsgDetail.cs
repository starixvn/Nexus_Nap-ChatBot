namespace Fanex.NCB.Shared.Models
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageMsgDetail
    {
        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("MsgTargetId")]
        public int MsgTargetId { get; set; }

        [XmlAttribute("BetTransId")]
        public string BetTransId { get; set; }

        [XmlAttribute("TargetId")]
        public int TargetId { get; set; }
    }
}
