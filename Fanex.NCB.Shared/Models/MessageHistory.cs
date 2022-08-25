namespace Fanex.NCB.Shared.Models
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageHistory
    {
        [XmlAttribute("MessageId")]
        public long MessageId { get; set; }

        [XmlAttribute("TargetId")]
        public int TargetId { get; set; }

        [XmlAttribute("RequestId")]
        public int RequestId { get; set; }
    }
}