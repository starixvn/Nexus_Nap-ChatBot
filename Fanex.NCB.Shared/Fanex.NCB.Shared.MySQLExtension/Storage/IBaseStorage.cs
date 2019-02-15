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
        Task<List<T>> QueryAsync<T>(string key, object param = null);

        Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(string key, object param = null);

        Task<ResultModel> ExecuteAsync(string key, DynamicParameters param);
    }
}
