namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets;

using System;
using System.Xml.Serialization;

[XmlType("r")]
public class AbnormalUsernames
{
    [XmlAttribute("VoidingHistoryCollectionId")]
    public int VoidingHistoryCollectionId { get; set; }

    [XmlAttribute("VoidingHistoryId")] public long VoidingHistoryId { get; set; }

    [XmlAttribute("Username")] public string Username { get; set; }

    [XmlAttribute("CustId")] public int CustId { get; set; }

    [XmlAttribute("MinWinlostDate")] public DateTime MinWinlostDate { get; set; }

    [XmlAttribute("MaxWinlostDate")] public DateTime MaxWinlostDate { get; set; }
}