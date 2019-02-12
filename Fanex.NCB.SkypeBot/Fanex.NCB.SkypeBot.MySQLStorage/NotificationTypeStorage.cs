using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Fanex.NCB.Common.Services;
using Fanex.NCB.SkypeBot.BaseStorage;
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
            var setting = _dBConfigService.Get("ChatBot_System_Sel");
            using (var dbConnection = new MySqlConnection(setting.DBConnection))
            {
                var data = dbConnection.Query<RecipientType>(
                    setting.SPName, 
                    null,
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }            
        }
    }
}
