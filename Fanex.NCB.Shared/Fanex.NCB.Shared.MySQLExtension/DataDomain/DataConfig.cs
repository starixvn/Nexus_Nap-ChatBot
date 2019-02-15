using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Shared.MySQLExtension.DataDomain
{
    public class DataConfig
    {
        public string DBName { get; set; }

        public string Connection { get; set; }

        public List<SPConfig> SPList { get; set; }

    }
}
