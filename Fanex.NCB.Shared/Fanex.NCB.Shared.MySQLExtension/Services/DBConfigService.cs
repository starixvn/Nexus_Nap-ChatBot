using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Fanex.NCB.Shared.MySQLExtension.DTO;

namespace Fanex.NCB.Shared.MySQLExtension.Services
{
    public class DBConfigService : IDBConfigService
    {
        private Dictionary<string, DBSetting> StoreProcedures { get; set; }

        public DBConfigService(string storedProcedurePath, ConnectionStrings connectionStrings)
        {
            StoreProcedures = new Dictionary<string, DBSetting>();
            XmlDocument doc = new XmlDocument();
            doc.Load(storedProcedurePath);

            foreach(XmlNode functionNode in doc.DocumentElement.ChildNodes)
            {
                foreach(XmlNode settingNode in functionNode.ChildNodes)
                {
                    var key = settingNode.Attributes["Key"]?.InnerText;
                    var sPName = settingNode.Attributes["SpName"]?.InnerText;
                    var dbName = settingNode.Attributes["DbName"]?.InnerText;                    
                    var setting = new DBSetting
                    {
                        Key = key,
                        SPName = sPName,
                        DBConnection = connectionStrings.GetType().GetProperty(dbName).GetValue(connectionStrings).ToString()
                    };

                    StoreProcedures.Add(key, setting);
                }
            }
        }

        public DBSetting Get(string key)
        {
            DBSetting result;
            StoreProcedures.TryGetValue(key, out result);

            return result;
        }
    }
}
