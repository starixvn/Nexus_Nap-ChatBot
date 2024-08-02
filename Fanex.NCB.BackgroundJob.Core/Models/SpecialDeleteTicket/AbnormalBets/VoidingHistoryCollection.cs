namespace Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.AbnormalBets
{
    using System.Xml.Serialization;

    public class VoidingHistoryCollection
    {
        [XmlAttribute("VoidingHistoryCollectionId")]
        public int VoidingHistoryCollectionId { get; set; }

        [XmlAttribute("VoidingHistoryId")]
        public int VoidingHistoryId { get; set; }
    }
}