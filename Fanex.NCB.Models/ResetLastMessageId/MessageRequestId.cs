namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageRequestId
    {
        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("RequestId")]
        public int RequestId { get; set; }
    }
}