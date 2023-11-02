namespace Fanex.NCB.Shared.Models.Settlement
{
    public class MessageSettlementDetail
    {
        public int MessageId { get; set; }

        public int TargetId { get; set; }

        public string Site { get; set; }

        public bool IsChinaView { get; set; }

        public byte SpecialPlatform { get; set; }

        public short LeagueGroupID { get; set; }
    }
}
