namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class CollectionTransId : VoidingHistoryCollection
    {
        [XmlAttribute("FilterBy")]
        public int FilterBy { get; set; }

        [XmlAttribute("TransIds")]
        public string TransIds { get; set; }

        [XmlAttribute("IssueTypeId")]
        public int IssueTypeId { get; set; }
    }
}