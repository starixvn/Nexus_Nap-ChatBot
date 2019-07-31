using System.Collections.Generic;

namespace Fanex.NCB.Shared.MySQLExtension.DataDomain
{
    public class DataConfig
    {
        public string ConnectionName { get; set; }

        public List<SPConfig> SPList { get; set; }
    }
}