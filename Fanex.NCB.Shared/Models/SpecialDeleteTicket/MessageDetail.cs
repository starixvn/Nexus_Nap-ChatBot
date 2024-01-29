namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageDetail
    {
        [XmlAttribute("MessageId")]
        public long MessageId { get; set; }

        [XmlAttribute("Site")]
        public string Site { get; set; }

        [XmlAttribute("TargetId")]
        public int TargetId { get; set; }

        [XmlAttribute("LeagueGroupId")]
        public int LeagueGroupId { get; set; }

        [XmlAttribute("IsChinaView")]
        public int IsChinaView { get; set; }

        [XmlAttribute("SpecialPlatform")]
        public byte SpecialPlatform { get; set; }
    }
}