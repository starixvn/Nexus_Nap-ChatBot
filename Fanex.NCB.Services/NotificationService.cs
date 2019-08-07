namespace Fanex.NCB.Services
{
    using Fanex.NCB.Criteria.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;

        public NotificationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> CreateAsync(CreateNotification model)
        {
            return await baseService.ExecuteNonQueryAsync(new CreateNewNotificationCommand
            {
                NotificationTypeId = model.NotificationTypeId,
                ProductId = model.ProductId,
                RecipientIDLst = model.RecipientIds,
                Title = model.Title,
                Content = model.Content,
                ConfigSendTime = model.SendDate,
                SendImmediately = model.SendImmediately,
                CreatorId = model.CreatorId
            }).ConfigureAwait(false);
        }
    }
}