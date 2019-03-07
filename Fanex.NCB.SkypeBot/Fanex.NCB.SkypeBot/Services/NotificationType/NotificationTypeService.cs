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
            var result = new NotificationTypeListResultModel
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

            return result;
        }

        public async Task<List<NotificationSystem>> GetNotificationSystemsAsync()
        {
            return await _notificationTypeStorage.GetNotificationSystemsAsync().ConfigureAwait(false);
        }

        public async Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return await _notificationTypeStorage.GetRecipientTypesAsync().ConfigureAwait(false);
        }

        public async Task<ResultModel> CreateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            return await _notificationTypeStorage.CreateNotificationTypeAsync(item).ConfigureAwait(false);
        }

        public async Task<ResultModel> UpdateNotificationTypeAsync(CreateNotificationTypeCriteria item)
        {
            return await _notificationTypeStorage.UpdateNotificationTypeAsync(item).ConfigureAwait(false);
        }

        public async Task<ResultModel> DeleteNotificationTypeAsync(int notificationTypeId)
        {
            return await _notificationTypeStorage.DeleteNotificationTypeAsync(notificationTypeId).ConfigureAwait(false);
        }
    }
}