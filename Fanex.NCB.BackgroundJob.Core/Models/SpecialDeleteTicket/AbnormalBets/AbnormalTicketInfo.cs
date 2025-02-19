namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets;

using System.Xml.Serialization;

[XmlType("r")]
public class AbnormalTicketInfo
{
    [XmlAttribute("VoidingHistoryCollectionId")]
    public int VoidingHistoryCollectionId { get; set; }

    [XmlAttribute("MessageId")] public long MessageId { get; set; }

    [XmlAttribute("Recommend")] public int Recommend { get; set; }

    [XmlAttribute("Bettype")] public int Bettype { get; set; }

    [XmlAttribute("LiveIndicator")] public bool LiveIndicator { get; set; }

    [XmlAttribute("IsParlay")] public bool IsParlay { get; set; }
}