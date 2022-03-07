namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    public class MessageDetail
    {
        public long MessageID { get; set; }

        public string Site { get; set; }

        public int TargetID { get; set; }

        public int LeagueGroupID { get; set; }

        public int IsChinaView { get; set; }
    }
}