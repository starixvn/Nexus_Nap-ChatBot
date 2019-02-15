using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Services;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.MySQLStorage.NotificationType
{
    public class NotificationTypeStorage : INotificationTypeStorage
    {
        private readonly IBaseStorage _baseStorage;

        public NotificationTypeStorage(IBaseStorage baseStorage)
        {
            _baseStorage = baseStorage;
        }

        public async Task<NotificationTypeListResultModel> GetNotificationTypeListAsync(int pageIndex, int pageSize)
        {

            var param = new { pageIndex, pageSize };
            var resultSets = await _baseStorage.QueryMultipleAsync<int, NotificationTypeItem>("ChatBot_NotificationType_Sel", param);

            return new NotificationTypeListResultModel { Count = resultSets.Item1, Data = resultSets.Item2 };
        }

        public async Task<List<NotificationSystem>> GetNotificationSystemsAsync()
        {
            return await _baseStorage.QueryAsync<NotificationSystem>("ChatBot_System_Sel");
        }

        public async Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return await _baseStorage.QueryAsync<RecipientType>("ChatBot_RecipientType_Sel");
        }

        public async Task<ResultModel> CreateNotificationTypeAsync(NotificationTypeItem item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("ChatBot_NotificationType_Ins", param);
        }

        public async Task<ResultModel> UpdateNotificationTypeAsync(NotificationTypeItem item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("id", value: item.NotificationTypeId, dbType: DbType.Int32);
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("ChatBot_NotificationType_Upd", param);
        }

        public async Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeId", value: notificationTypeId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("ChatBot_NotificationType_Del", param);
        }
    }
}
