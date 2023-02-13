namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class CollectionUsername : VoidingHistoryCollection
    {
        [XmlAttribute("Usernames")]
        public string Usernames { get; set; }
    }
}