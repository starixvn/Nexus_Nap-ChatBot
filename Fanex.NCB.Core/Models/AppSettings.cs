namespace Fanex.NCB.Core.Models
{
    public class AppSettings
    {
        public int SiteId { get; set; }

        public int ItemEachBatchSendNotification { get; set; }

        public int HttpClientTimeout { get; set; }

        public string AppCredentialId { get; set; }

        public string AppCredentialPassword { get; set; }

        public string SecretKey { get; set; }
        public int MessageContentLength { get; set; }
    }
}