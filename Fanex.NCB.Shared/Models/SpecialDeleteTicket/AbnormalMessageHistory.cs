namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalMessageHistory
    {
        [XmlAttribute("MessageID")]
        public int MessageId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public int VoidingHistoryId { get; set; }

        [XmlAttribute("UsernameLst")]
        public string UsernameLst { get; set; }
    }
}