namespace Fanex.NCB.Shared.Models.SiteMessage
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageHistory
    {
        [XmlAttribute("MessageID")]
        public long MessageID { get; set; }

        [XmlAttribute("TargetID")]
        public int TargetID { get; set; }
    }
}