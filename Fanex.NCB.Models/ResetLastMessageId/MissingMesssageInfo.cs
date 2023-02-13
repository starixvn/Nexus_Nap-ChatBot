namespace Fanex.NCB.Models.ResetLastMessageId
{
    using System;

    public class MissingMesssageInfo
    {
        public int VoidingHistoryCollectionId { get; set; }
        public string MessageIds { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }
        public int JobIdentify { get; set; }
        public int MessageId { get; set; }
    }
}