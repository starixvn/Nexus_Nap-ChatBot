using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Services;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
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

        public async Task<Tuple<int, List<NotificationTypeItem>>> GetNotificationTypeListAsync(int pageIndex, int pageSize)
        {

            var param = new { pageIndex, pageSize };
            return await _baseStorage.QueryMultipleAsync<int, NotificationTypeItem>("NCB_NotificationType_Sel", param).ConfigureAwait(false);

            //return new NotificationTypeListResultModel { Count = resultSets.Item1, Data = resultSets.Item2 };
        }

        public async Task<List<NotificationSystem>> GetNotificationSystemsAsync()
        {
            return await _baseStorage.QueryAsync<NotificationSystem>("NCB_System_Sel").ConfigureAwait(false);
        }

        public async Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return await _baseStorage.QueryAsync<RecipientType>("NCB_RecipientType_Sel").ConfigureAwait(false);
        }

        public async Task<ResultModel> CreateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeIds", value: item.RecipientTypeIds, dbType: DbType.String);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("NCB_NotificationType_Ins", param).ConfigureAwait(false);
        }

        public async Task<ResultModel> UpdateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("id", value: item.NotificationTypeId, dbType: DbType.Int32);
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeIds", value: item.RecipientTypeIds, dbType: DbType.String);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("NCB_NotificationType_Upd", param).ConfigureAwait(false);
        }

        public async Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeId", value: notificationTypeId, dbType: DbType.Int32);

            return await _baseStorage.ExecuteAsync("NCB_NotificationType_Del", param).ConfigureAwait(false);
        }
    }
}
