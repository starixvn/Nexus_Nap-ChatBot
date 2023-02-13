namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
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

        [XmlAttribute("Usernames")]
        public string Usernames { get; set; }
    }
}