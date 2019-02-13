using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Fanex.NCB.Common.Services;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using MySql.Data.MySqlClient;

namespace Fanex.NCB.SkypeBot.MySQLStorage
{
    public class NotificationTypeStorage : INotificationTypeStorage
    {
        private readonly IDBConfigService _dBConfigService;

        public NotificationTypeStorage(IDBConfigService dBConfigService)
        {
            _dBConfigService = dBConfigService;
        }

        public NotificationTypeListResultModel GetNotificationTypeList(int pageIndex, int pageSize)
        {
            var setting = _dBConfigService.Get("ChatBot_NotificationType_Sel");            
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                var reader = dbConnection.QueryMultiple(
                    setting.SPName,
                    param: new { pageIndex, pageSize },
                    commandType: CommandType.StoredProcedure);

                var total = reader.Read<int>().First();
                var data = reader.Read<NotificationTypeItem>().ToList();

                return new NotificationTypeListResultModel { Count = total, Data = data };
            }
        }

        public List<NotificationSystem> GetNotificationSystems()
        {
            var setting = _dBConfigService.Get("ChatBot_System_Sel");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                var data = dbConnection.Query<NotificationSystem>(
                    setting.SPName,
                    null,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
        }

        public List<RecipientType> GetRecipientTypes()
        {
            var setting = _dBConfigService.Get("ChatBot_RecipientType_Sel");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                var data = dbConnection.Query<RecipientType>(
                    setting.SPName, 
                    null,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }            
        }

        public ResultModel CreateNotificationType(NotificationTypeItem item)
        {
            var setting = _dBConfigService.Get("ChatBot_NotificationType_Ins");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
                param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
                param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);
                param.Add("errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("errorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);
               
                dbConnection.Execute(
                    setting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return new ResultModel {
                    ErrorCode = param.Get<int>("errorCode"),
                    ErrorMsg = param.Get<string>("errorMsg")
                };
            }
        }

        public ResultModel UpdateNotificationType(NotificationTypeItem item)
        {
            var setting = _dBConfigService.Get("ChatBot_NotificationType_Upd");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("id", value: item.NotificationTypeId, dbType: DbType.Int32);
                param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
                param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
                param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);
                param.Add("errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("errorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                dbConnection.Execute(
                    setting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return new ResultModel
                {
                    ErrorCode = param.Get<int>("errorCode"),
                    ErrorMsg = param.Get<string>("errorMsg")
                };
            }
        }

        public ResultModel DeleteNotificationType(int notificationTypeId)
        {
            var setting = _dBConfigService.Get("ChatBot_NotificationType_Del");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("notificationTypeId", value: notificationTypeId, dbType: DbType.Int32);                
                param.Add("errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("errorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                dbConnection.Execute(
                    setting.SPName,
                    param: param,
                    commandType: CommandType.StoredProcedure);

                return new ResultModel
                {
                    ErrorCode = param.Get<int>("errorCode"),
                    ErrorMsg = param.Get<string>("errorMsg")
                };
            }
        }
    }
}
