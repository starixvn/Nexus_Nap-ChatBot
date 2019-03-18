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
        Task<List<T>> QueryAsync<T>(string key, object param = null, string dbName = "ChatBotDB");

        Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(string key, object param = null, string dbName = "ChatBotDB");

        Task<Tuple<T1, List<T2>, List<T3>, List<T4>>> QueryMultipleAsync<T1, T2, T3, T4>(string key, object param = null, string dbName = "ChatBotDB");

        Task<ResultModel> ExecuteAsync(string key, DynamicParameters param, string dbName = "ChatBotDB");
    }
}
