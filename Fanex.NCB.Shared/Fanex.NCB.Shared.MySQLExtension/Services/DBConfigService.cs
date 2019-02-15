using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Fanex.NCB.Shared.MySQLExtension.DataDomain;
using Fanex.NCB.Shared.MySQLExtension.DTO;
using Microsoft.Extensions.Options;

namespace Fanex.NCB.Shared.MySQLExtension.Services
{
    public class DBConfigService : IDBConfigService
    {
        private readonly List<DataConfig> _spNames;

        public DBConfigService(IOptionsSnapshot<List<DataConfig>> spNames)
        {
            _spNames = spNames.Value;
        }        

        public DBSetting GetSPConfig(string dbName, string spKey)
        {
            var db = _spNames.First(s => s.DBName == dbName);
            return new DBSetting
            {
                Key = spKey,
                DBConnection = db.Connection,
                SPName = db.SPList.FirstOrDefault(i => i.SPKey == spKey).SPName
            };
        }
    }
}
