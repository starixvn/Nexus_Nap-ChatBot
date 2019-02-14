using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.Services;
using Fanex.NCB.Common.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fanex.NCB.SkypeBot.MySQLStorage.NotificationType
{
    public class NotificationTypeStorage : INotificationTypeStorage
    {
        private readonly IDBConfigService _dBConfigService;
        private readonly IBaseStorage _baseStorage;

        public NotificationTypeStorage(IDBConfigService dBConfigService, IBaseStorage baseStorage)
        {
            _dBConfigService = dBConfigService;
            _baseStorage = baseStorage;
        }

        public NotificationTypeListResultModel GetNotificationTypeList(int pageIndex, int pageSize)
        {
            var dbSetting = _dBConfigService.Get("ChatBot_NotificationType_Sel");
            var param = new { pageIndex, pageSize };
            var resultSets = _baseStorage.QueryMultiple<int, NotificationTypeItem>(param, dbSetting);

            return new NotificationTypeListResultModel { Count = resultSets.Item1, Data = resultSets.Item2 };
        }

        public List<NotificationSystem> GetNotificationSystems()
        {
            var dbSetting = _dBConfigService.Get("ChatBot_System_Sel");

            return _baseStorage.Query<NotificationSystem>(null, dbSetting);
        }

        public List<RecipientType> GetRecipientTypes()
        {
            var dbSetting = _dBConfigService.Get("ChatBot_RecipientType_Sel");

            return _baseStorage.Query<RecipientType>(null, dbSetting);
        }

        public ResultModel CreateNotificationType(NotificationTypeItem item)
        {
            var dbSetting = _dBConfigService.Get("ChatBot_NotificationType_Ins");
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return _baseStorage.Execute(param, dbSetting);
        }

        public ResultModel UpdateNotificationType(NotificationTypeItem item)
        {
            var dbSetting = _dBConfigService.Get("ChatBot_NotificationType_Upd");
            DynamicParameters param = new DynamicParameters();
            param.Add("id", value: item.NotificationTypeId, dbType: DbType.Int32);
            param.Add("notificationTypeName", value: item.NotificationTypeName, dbType: DbType.String);
            param.Add("recipientTypeId", value: item.RecipientTypeId, dbType: DbType.Int32);
            param.Add("systemId", value: item.SystemId, dbType: DbType.Int32);

            return _baseStorage.Execute(param, dbSetting);
        }

        public ResultModel DeleteNotificationType(int notificationTypeId)
        {
            var dbSetting = _dBConfigService.Get("ChatBot_NotificationType_Del");
            DynamicParameters param = new DynamicParameters();
            param.Add("notificationTypeId", value: notificationTypeId, dbType: DbType.Int32);

            return _baseStorage.Execute(param, dbSetting);
        }
    }
}
