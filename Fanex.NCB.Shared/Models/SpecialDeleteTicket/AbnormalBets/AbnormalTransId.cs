namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalTransId
    {
        [XmlAttribute("MessageId")]
        public long MessageId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public long VoidingHistoryId { get; set; }

        [XmlAttribute("TransIds")]
        public string TransIds { get; set; }

        [XmlAttribute("FilterBy")]
        public int FilterBy { get; set; }

        [XmlAttribute("IssueTypeId")]
        public int IssueTypeId { get; set; }
    }
}