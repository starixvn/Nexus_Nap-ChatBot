namespace Fanex.NCB.AutoJobs.Core.Models.Common
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class MessageMsgDetail
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("MsgTargetId")]
        public int MsgTargetId { get; set; }

        [XmlAttribute("BetTransId")]
        public string BetTransId { get; set; }

        [XmlAttribute("TargetId")]
        public int TargetId { get; set; }

        [XmlAttribute("LeagueGroupId")]
        public int LeagueGroupId { get; set; }
    }
}