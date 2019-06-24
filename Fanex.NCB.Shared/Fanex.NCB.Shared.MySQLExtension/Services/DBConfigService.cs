using Fanex.NCB.Shared.MySQLExtension.DataDomain;
using Fanex.NCB.Shared.MySQLExtension.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NCB.Shared.MySQLExtension.Services
{
    public class DBConfigService : IDBConfigService
    {
        private readonly List<DataConfig> _spNames;
        private readonly IConfiguration _configuration;

        public DBConfigService(IOptionsSnapshot<List<DataConfig>> spNames, IConfiguration configuration)
        {
            _spNames = spNames.Value;
            _configuration = configuration;
        }

        public DBSetting GetSPConfig(string connectionName, string spKey)
        {
            var db = _spNames.Find(s => s.ConnectionName == connectionName);
            var connectionString = _configuration.GetConnectionString(db?.ConnectionName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException($"Not found connection '{connectionName}' in app setting");
            }

            if (db?.SPList.Any(i => i.SPKey == spKey) != true)
            {
                throw new ArgumentException($"connectionName={connectionName}, Key={spKey}, Msg=Not found {spKey} in config file");
            }

            return new DBSetting
            {
                Key = spKey,
                DBConnection = connectionString,
                SPName = db.SPList.Find(i => i.SPKey == spKey)?.SPName
            };
        }
    }
}