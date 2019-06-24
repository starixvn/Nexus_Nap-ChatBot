using Dapper;
using Fanex.NCB.Common.Constant;
using Fanex.NCB.Common.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NCB.Shared.MySQLExtension.Storage
{
    public interface IBaseStorage
    {
        Task<List<T>> QueryAsync<T>(string key, object param = null, string connectionName = Default.CONNECTION_NAME);

        Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(string key, object param = null, string connectionName = Default.CONNECTION_NAME);

        Task<Tuple<T1, List<T2>, List<T3>, List<T4>>> QueryMultipleAsync<T1, T2, T3, T4>(string key, object param = null, string connectionName = Default.CONNECTION_NAME););

        Task<ResultModel> ExecuteAsync(string key, DynamicParameters param, string connectionName = Default.CONNECTION_NAME);
    }
}