using Fanex.NCB.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Common.Services
{
    public interface IDBConfigService
    {
        DBSetting Get(string key);
    }
}
