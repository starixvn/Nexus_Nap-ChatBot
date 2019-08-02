namespace Fanex.NCB.Core.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public int SiteId { get; set; }

        public string Env { get; set; }

        public string TokenUrl { get; set; }

        public string SkypeServiceUrl { get; set; }
    }
}