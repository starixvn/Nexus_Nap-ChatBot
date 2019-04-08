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
            try {
                var db = _spNames.FirstOrDefault(s => s.DBName == dbName);
                var dbsetting = new DBSetting
                {
                    Key = spKey,
                    DBConnection = db.Connection,
                    SPName = db.SPList.FirstOrDefault(i => i.SPKey == spKey).SPName
                };

                if (dbsetting.SPName == null)
                {
                    throw new ArgumentException($"DBName={dbName}, Key={spKey}, msg={"SP config not found"}");
                }

                return dbsetting;
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"DBName={dbName}, Key={spKey}, msg={"SP config not found"}");
            }
        }
    }
}
