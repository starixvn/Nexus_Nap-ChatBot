using Fanex.NCB.Shared.MySQLExtension.DTO;

namespace Fanex.NCB.Shared.MySQLExtension.Services
{
    public interface IDBConfigService
    {
        DBSetting GetSPConfig(string connectionName, string spKey);
    }
}