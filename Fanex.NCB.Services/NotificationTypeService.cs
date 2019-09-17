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

        public Task<CommandResult> InsertAsync(NotificationTypeViewModel model)
        {
            var command = new InsertNotificationTypeCommand
            {
                NotificationTypeName = model.NotificationTypeName,
                SystemId = model.SystemId,
                RecipientTypeIds = model.RecipientTypeIds,
                ChatGroupTypeIds = model.ChatGroupTypeIds,
                UserId = model.UserId
            };

            return baseService.ExecuteNonQueryAsync(command);
        }

        public Task<CommandResult> UpdateAsync(NotificationTypeViewModel model)
        {
            var command = new UpdateNotificationTypeCommand
            {
                NotificationTypeId = model.NotificationTypeId,
                NotificationTypeName = model.NotificationTypeName,
                RecipientTypeIds = model.RecipientTypeIds,
                ChatGroupTypeIds = model.ChatGroupTypeIds,
                SystemId = model.SystemId,
                UserId = model.UserId
            };

            return baseService.ExecuteNonQueryAsync(command);
        }

        public Task<CommandResult> DeleteAsync(int notificationTypeId, int userId)
        {
            var command = new DeleteNotificationTypeCommand
            {
                NotificationTypeId = notificationTypeId,
                UserId = userId
            };

            return baseService.ExecuteNonQueryAsync(command);
        }
    }
}