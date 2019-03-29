using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Model.DTO;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.NotificationType
{
    public class NotificationTypeService : INotificationTypeService
    {
        private readonly INotificationTypeStorage _notificationTypeStorage;
        public NotificationTypeService(INotificationTypeStorage notificationTypeStorage)
        {
            _notificationTypeStorage = notificationTypeStorage;
        }

        public async Task<NotificationTypeListResultModel> GetNotificationTypeListAsync(int pageIndex, int pageSize)
        {
            var data = await _notificationTypeStorage.GetNotificationTypeListAsync(pageIndex, pageSize).ConfigureAwait(false);
            return  new NotificationTypeListResultModel
            {
                Count = data.Item1,
                Data = data.Item2.GroupBy(i => i.NotificationTypeId).Select(g =>
                {
                    return new Model.DTO.NotificationType.NotificationType
                    {
                        NotificationTypeId = g.First().NotificationTypeId,
                        NotificationTypeName = g.First().NotificationTypeName,
                        SystemId = g.First().SystemId,
                        SystemName = g.First().SystemName,
                        RecipientTypes = g.Select(i => new RecipientType { Id = i.RecipientTypeId, Name = i.RecipientTypeName }).ToList()
                    };
                }).ToList()
            };

        }

        public Task<List<NotificationSystem>> GetNotificationSystemsAsync()
        {
            return _notificationTypeStorage.GetNotificationSystemsAsync();
        }

        public Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return _notificationTypeStorage.GetRecipientTypesAsync();
        }

        public Task<ResultModel> CreateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            return _notificationTypeStorage.CreateNotificationTypeAsync(item);
        }

        public Task<ResultModel> UpdateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            return _notificationTypeStorage.UpdateNotificationTypeAsync(item);
        }

        public Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId)
        {
            return _notificationTypeStorage.DeleteNotificationTypeAsync(notificationTypeId);
        }
    }
}