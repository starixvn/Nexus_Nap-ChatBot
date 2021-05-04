namespace Fanex.NCB.Shared.Models
{
    using System.Collections.Generic;
    using System.Net;

    public class NotificationMessageResponse
    {
        public bool IsSuccessStatusCode { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public IEnumerable<SendMessageResult> NotificationMessageResult { get; set; }
    }
}
