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

        public async Task<ResultModel> ExecuteAsync(string key, DynamicParameters param)
        {
            DBSetting dbSetting = _dBConfigService.Get(key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                param.Add("errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("errorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                await dbConnection.ExecuteAsync(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return new ResultModel
                {
                    ErrorCode = param.Get<int>("errorCode"),
                    ErrorMsg = param.Get<string>("errorMsg")
                };
            }
        }

        public async Task<List<T>> QueryAsync<T>(string key, object param = null)
        {
            DBSetting dbSetting = _dBConfigService.Get(key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var data = await dbConnection.QueryAsync<T>(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
        }
      
        public async Task<Tuple<T1, List<T2>>> QueryMultipleAsync<T1, T2>(string key, object param = null)
        {
            DBSetting dbSetting = _dBConfigService.Get(key);
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var reader = await dbConnection.QueryMultipleAsync(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                var total = reader.Read<T1>().First();
                var data = reader.Read<T2>().ToList();

                return Tuple.Create(total, data);
            }
        }
    }
}
