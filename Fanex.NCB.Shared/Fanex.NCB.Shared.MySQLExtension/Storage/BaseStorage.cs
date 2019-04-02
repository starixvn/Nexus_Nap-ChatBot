using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.DTO;
using Fanex.NCB.Shared.MySQLExtension.Services;
using MySql.Data.MySqlClient;

namespace Fanex.NCB.Shared.MySQLExtension.Storage
{
    public class BaseStorage : IBaseStorage
    {
        private readonly IDBConfigService _dBConfigService;

        public BaseStorage(IDBConfigService dBConfigService)
        {
            _dBConfigService = dBConfigService;
        }

        public async Task<ResultModel> ExecuteAsync(string key, DynamicParameters param, string dbName = "ChatBotDB")
        {
            DBSetting dbSetting = _dBConfigService.GetSPConfig(dbName, key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                param.Add("out_ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("out_ErrorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                await dbConnection.ExecuteAsync(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return new ResultModel
                {
                    ErrorCode = param.Get<int>("out_ErrorCode"),
                    ErrorMsg = param.Get<string>("out_ErrorMsg")
                };
            }
        }

        public async Task<List<T>> QueryAsync<T>(string key, object param = null, string dbName = "ChatBotDB")
        {
            DBSetting dbSetting = _dBConfigService.GetSPConfig(dbName, key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var data = await dbConnection.QueryAsync<T>(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return data.ToList();
            }
        }

        public async Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(string key, object param = null, string dbName = "ChatBotDB")
        {
            DBSetting dbSetting = _dBConfigService.GetSPConfig(dbName, key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var reader = await dbConnection.QueryMultipleAsync(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                var total = await reader.ReadAsync<T1>().ConfigureAwait(false);
                var data = await reader.ReadAsync<T2>().ConfigureAwait(false);

                return Tuple.Create(total.First(), data.ToList());
            }
        }

        public async Task<Tuple<T1, List<T2>, List<T3>, List<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(string key, object param = null, string dbName = "ChatBotDB")
        {
            DBSetting dbSetting = _dBConfigService.GetSPConfig(dbName, key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var reader = await dbConnection.QueryMultipleAsync(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                var total = await reader.ReadAsync<T1>().ConfigureAwait(false);
                var data1 = await reader.ReadAsync<T2>().ConfigureAwait(false);
                var data2 = await reader.ReadAsync<T3>().ConfigureAwait(false);
                var data3 = await reader.ReadAsync<T4>().ConfigureAwait(false);

                return Tuple.Create(total.First(), data1.ToList(), data2.ToList(), data3.ToList());
            }
        }
    }
}
