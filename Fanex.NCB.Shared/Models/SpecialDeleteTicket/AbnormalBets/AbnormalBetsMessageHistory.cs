namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalBetsMessageHistory
    {
        [XmlAttribute("MessageId")]
        public long MessageId { get; set; }

        [XmlAttribute("RequestId")]
        public int RequestId { get; set; }
    }
}