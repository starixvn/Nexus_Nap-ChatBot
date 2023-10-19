namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System;
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalUsernames
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("MessageId")]
        public long MessageId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public long VoidingHistoryId { get; set; }

        [XmlAttribute("Username")]
        public string Username { get; set; }

        [XmlAttribute("CustId")]
        public int CustId { get; set; }

        [XmlAttribute("MinWinLostDate")]
        public DateTime MinWinLostDate { get; set; }

        [XmlAttribute("MaxWinLostDate")]
        public DateTime MaxWinLostDate { get; set; }
    }
}