using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Fanex.NCB.Common.DTO;
using MySql.Data.MySqlClient;

namespace Fanex.NCB.Common.Storage
{
    public class BaseStorage : IBaseStorage
    {
        public ResultModel Execute(DynamicParameters param, DBSetting dbSetting)
        {
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {               
                param.Add("errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("errorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                dbConnection.Execute(
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

        public List<T> Query<T>(object param, DBSetting dbSetting)
        {
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var data = dbConnection.Query<T>(
                    dbSetting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
        }

        public Tuple<T1, List<T2>> QueryMultiple<T1, T2>(object param, DBSetting dbSetting)
        {
            using (var dbConnection = new MySqlConnection(dbSetting.DBConnection))
            {
                var reader = dbConnection.QueryMultiple(
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
