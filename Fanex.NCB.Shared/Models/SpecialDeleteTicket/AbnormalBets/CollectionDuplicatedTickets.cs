namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class CollectionDuplicatedTickets : VoidingHistoryCollection
    {
        [XmlAttribute("DuplicatedTickets")]
        public string DuplicatedTickets { get; set; }

        [XmlAttribute("FilterBy")]
        public int FilterBy { get; set; }

        [XmlAttribute("IssueTypeId")]
        public int IssueTypeId { get; set; }
    }
}