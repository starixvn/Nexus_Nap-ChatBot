namespace Fanex.NCB.AutoJobs.Core.Models.Settlement
{
    public class MessageSettlementDetail
    {
        public int MessageId { get; set; }

        public int TargetId { get; set; }

        public string Site { get; set; }

        public bool IsChinaView { get; set; }

        public byte SpecialPlatform { get; set; }

        public short LeagueGroupId { get; set; }
    }
}
