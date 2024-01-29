namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    public class MessageDetail
    {
        public long MessageId { get; set; }

        public string Site { get; set; }

        public int TargetId { get; set; }

        public int LeagueGroupId { get; set; }

        public int IsChinaView { get; set; }

        public byte SpecialPlatform { get; set; }
    }
}