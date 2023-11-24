namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalMessageContent
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("Content_en")]
        public string Content_en { get; set; }

        [XmlAttribute("Content_cs")]
        public string Content_cs { get; set; }

        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("CreatorName")]
        public string CreatorName { get; set; }
    }
}