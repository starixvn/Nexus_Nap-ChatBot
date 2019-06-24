namespace Fanex.NCB.Core.Models.DataDomain
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int SiteId { get; set; }
        public string Env { get; set; }
        public string AuthApi { get; set; }
        public string UserResourceApi { get; set; }
        public string TokenUrl { get; set; }
        public string SkypeServiceUrl { get; set; }
        public string StoredProcedurePath { get; set; }
    }
}