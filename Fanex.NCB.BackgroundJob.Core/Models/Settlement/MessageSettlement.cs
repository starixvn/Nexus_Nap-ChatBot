namespace Fanex.NCB.AutoJobs.Core.Models.Settlement;

using System.Xml.Serialization;

[XmlType("r")]
public class MessageSettlement
{
    [XmlAttribute("MessageId")] public int MessageId { get; set; }

    [XmlAttribute("MsgContentsType")] public int MsgContentsType { get; set; }

    [XmlAttribute("MatchId")] public int MatchId { get; set; }

    [XmlAttribute("Content_en")] public string Content_en { get; set; }

    [XmlAttribute("Content_cs")] public string Content_cs { get; set; }
}