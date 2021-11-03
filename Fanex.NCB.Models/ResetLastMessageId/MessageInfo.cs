namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System;

    public class MessageInfo
    {
        public int JobIdentify { get; set; }

        public int FromMessageId { get; set; }

        public int ToMessageId { get; set; }

        public DateTime PostTime { get; set; }
    }
}
