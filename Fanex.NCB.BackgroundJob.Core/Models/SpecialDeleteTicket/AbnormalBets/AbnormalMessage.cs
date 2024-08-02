namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    [XmlType("r")]
    public class AbnormalMessage
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("MessageId")]
        public int MessageId { get; set; }

        [XmlAttribute("SiteNameGroup")]
        public string SiteNameGroup { get; set; }
    }
}