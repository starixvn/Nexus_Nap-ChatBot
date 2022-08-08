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

        [XmlAttribute("TransId")]
        public string TransId { get; set; }

        [XmlAttribute("FilterBy")]
        public int FilterBy { get; set; }

        [XmlAttribute("IssueTypeId")]
        public int IssueTypeId { get; set; }
    }
}