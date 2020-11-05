namespace Fanex.NCB.Shared.Models.SpecialDeleteTicket
{
    using System;

    public class Message
    {
        public int MessageID { get; set; }

        public int MsgContentsType { get; set; }

        public string Content_en { get; set; }

        public string Content_cs { get; set; }

        public int RequestID { get; set; }

        public bool IsNew { get; set; }

        public DateTime? PostTime { get; set; }
    }
}