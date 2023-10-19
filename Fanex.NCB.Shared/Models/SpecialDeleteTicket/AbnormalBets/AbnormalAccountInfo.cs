namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System;
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalAccountInfo
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("Username")]
        public string Username { get; set; }

        [XmlAttribute("CustId")]
        public int CustId { get; set; }

        [XmlAttribute("MinWinLostDate")]
        public DateTime MinWinLostDate { get; set; }

        [XmlAttribute("MaxWinLostDate")]
        public DateTime MaxWinLostDate { get; set; }

        [XmlAttribute("SiteName")]
        public string SiteName { get; set; }

        [XmlAttribute("IsDeposit")]
        public bool IsDeposit { get; set; }

        [XmlAttribute("IsTest")]
        public bool IsTest { get; set; }
    }
}
