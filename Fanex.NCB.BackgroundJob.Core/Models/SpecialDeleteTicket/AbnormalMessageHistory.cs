namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalMessageHistory
    {
        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public int VoidingHistoryId { get; set; }

        [XmlAttribute("UsernameLst")]
        public string UsernameLst { get; set; }
    }
}