namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class DuplicateTicketDetail
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("OrgTransId")]
        public long OrgTransId { get; set; }

        [XmlAttribute("VoidedTransId")]
        public long VoidedTransId { get; set; }

        [XmlAttribute("CustId")]
        public int CustId { get; set; }

        [XmlAttribute("Username")]
        public string Username { get; set; }

        [XmlAttribute("CurrencyName")]
        public string CurrencyName { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("Stake")]
        public decimal Stake { get; set; }
    }
}