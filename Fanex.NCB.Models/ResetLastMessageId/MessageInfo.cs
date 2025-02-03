namespace Fanex.NCB.Models.ResetLastMessageId;

using System;
using System.Xml.Serialization;

[XmlType("r")]
public class MessageInfo
{
    [XmlAttribute("JobIdentify")] public int JobIdentify { get; set; }

    [XmlAttribute("FromMessageId")] public int FromMessageId { get; set; }

    [XmlAttribute("ToMessageId")] public int ToMessageId { get; set; }

    [XmlAttribute("PostTime")] public DateTime PostTime { get; set; }
}