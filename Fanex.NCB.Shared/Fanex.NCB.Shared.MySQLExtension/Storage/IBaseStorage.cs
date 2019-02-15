using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Shared.MySQLExtension.Storage
{
    public interface IBaseStorage
    {
        Task<List<T>> QueryAsync<T>(object param, DBSetting dbSetting);

        Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(object param, DBSetting dbSetting);

        Task<ResultModel> ExecuteAsync(DynamicParameters param, DBSetting dbSetting);
    }
}
