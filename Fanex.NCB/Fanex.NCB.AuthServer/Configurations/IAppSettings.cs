namespace Fanex.NCB.AuthServer.Configurations
{
    public interface IAppSettings
    {
        int SiteId { get; }

        string Environment { get; }
    }
}
