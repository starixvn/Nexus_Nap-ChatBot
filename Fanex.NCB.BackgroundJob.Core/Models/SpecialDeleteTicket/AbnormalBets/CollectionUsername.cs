namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets;

using System.Xml.Serialization;

[XmlType("r")]
public class CollectionUsername : VoidingHistoryCollection
{
    [XmlAttribute("Usernames")] public string Usernames { get; set; }
}