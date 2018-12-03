using Fanex.NAP.Core.Configuration;

namespace Fanex.NCB.AuthServer.Configurations
{
    public class AppSettings : IAppSettings
    {
        public int SiteId
        {
            get { return NapSettings.Settings.SiteId; }
        }

        public string Environment
        {
            get { return NapSettings.Settings.Environment; }
        }
    }
}