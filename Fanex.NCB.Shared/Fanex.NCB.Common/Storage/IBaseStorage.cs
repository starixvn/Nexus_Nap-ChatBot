using Dapper;
using Fanex.NCB.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Common.Storage
{
    public interface IBaseStorage
    {
        List<T> Query<T>(object param, DBSetting dbSetting);

        Tuple<T1, List<T2>> QueryMultiple<T1, T2>(object param, DBSetting dbSetting);

        ResultModel Execute(DynamicParameters param, DBSetting dbSetting);
    }
}
