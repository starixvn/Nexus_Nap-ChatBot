using Fanex.NCB.Shared.MySQLExtension.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Shared.MySQLExtension.Services
{
    public interface IDBConfigService
    {
        DBSetting GetSPConfig(string dbName, string spKey);
    }
}
