namespace Fanex.NCB.AutoJobs.Core.Models.Settlement
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageSettlementInfo
    {
        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("MsgTargetId")]
        public int MsgTargetId { get; set; }

        [XmlAttribute("BetTransId")]
        public string BetTransId { get; set; }

        [XmlAttribute("TargetId")]
        public int TargetId { get; set; }
    }
}
