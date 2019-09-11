namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.NotificationType;
    using Fanex.NCB.Models.NotificationType;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class NotificationTypeService : INotificationTypeService
    {
        private readonly IBaseService baseService;

        public NotificationTypeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> DeleteAsync(int notificationTypeId)
        {
            var command = new DeleteNotificationTypeCommand
            {
                NotificationId = notificationTypeId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        public async Task<(IEnumerable<NotificationType>, long)> GetAllAsync(int pageIndex, int pageSize)
        {
            var criteria = new GetNotificationTypeCriteria
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await baseService.FetchMultipleAsync<NotificationType, NotificationRecipientType, NotificationChatGroupType>(criteria).ConfigureAwait(false);

            foreach (var notificationType in data.Item1)
            {
                notificationType.RecipientTypes = data.Item2.Where(t => t.NotificationTypeId == notificationType.Id);
                notificationType.ChatGroupTypes = data.Item3.Where(t => t.NotificationTypeId == notificationType.Id);
            }

            return new ValueTuple<IEnumerable<NotificationType>, long>(data.Item1, criteria.TotalItems);
        }

        public async Task<IEnumerable<NotificationRecipientType>> GetRecipientType()
        {
            var criteria = new GetNotificationRecipientTypeCriteria();

            return await baseService.GetDataAsync<NotificationRecipientType>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<NotificationChatGroupType>> GetChatGroupType()
        {
            var criteria = new GetNotificationChatGroupTypeCriteria();

            return await baseService.GetDataAsync<NotificationChatGroupType>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BaseModel>> GetSystem()
        {
            var criteria = new GetNotificationTypeSystemCriteria();

            return await baseService.GetDataAsync<BaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> InsertAsync(NotificationType model)
        {
            var command = new InsertNotificationTypeCommand
            {
                NotificationTypeName = model.Name,
                SystemId = model.SystemId,
                RecipientTypeIds = string.Join(",", model.RecipientTypes.Select(x => x.Id))
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        public async Task<CommandResult> UpdateAsync(NotificationType model)
        {
            var command = new UpdateNotificationTypeCommand
            {
                NotificationTypeId = model.Id,
                NotificationTypeName = model.Name,
                SystemId = model.SystemId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }
    }
}