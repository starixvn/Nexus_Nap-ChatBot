namespace Fanex.NCB.Shared.Models
{
    using System.Collections.Generic;

    public class ReciveProactiveMessage
    {
        public int NotifictaionId { get; set; }

        public string Message { get; set; }

        public IEnumerable<ProactiveMessage> Recipients { get; set; }
    }
}