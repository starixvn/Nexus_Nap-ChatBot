namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System;
    using System.Xml.Serialization;

    [XmlType("r")]
    public class DuplicatedTicketInfo
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public long VoidingHistoryId { get; set; }

        [XmlAttribute("CustId")]
        public int CustId { get; set; }

        [XmlAttribute("OrgTransId")]
        public long OrgTransId { get; set; }

        [XmlAttribute("VoidedTransId")]
        public long VoidedTransId { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("Stake")]
        public decimal Stake { get; set; }

        [XmlAttribute("IsParlay")]
        public bool IsParlay { get; set; }

        [XmlAttribute("Transdate")]
        public DateTime Transdate { get; set; }
    }
}