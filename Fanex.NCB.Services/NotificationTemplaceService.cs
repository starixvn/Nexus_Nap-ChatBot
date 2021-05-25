namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.NotificationTemplate;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class NotificationTemplaceService : INotificationTemplaceService
    {
        private readonly IBaseService baseService;

        public NotificationTemplaceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<(long, IEnumerable<NotificationTemplate>)> GetAsync(int? notificationTypeId, int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<long, NotificationTemplate>(new GetTemplatesCriteria
            {
                NotificationTypeId = notificationTypeId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<CommandResult> CreateAsync(CreateNotificationTemplate model)
        {
            var command = new CreateNotificationTemplateCommand
            {
                NotificationTypeId = model.NotificationTypeId,
                NotificationTemplateName = model.NotificationTemplateName,
                Title = model.Title,
                ContentLst = model.Content,
                CreatorId = model.CreatorId
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> EditAsync(EditNotificationTemplate model)
        {
            var command = new EditNotificationTemplateCommand
            {
                NotificationTemplateId = model.NotificationTemplateId,
                NotificationTypeId = model.NotificationTypeId,
                NotificationTemplateName = model.NotificationTemplateName,
                Title = model.Title,
                ContentLst = model.Content,
                ModifierId = model.ModifierId
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<NotificationTemplate> GetByIdAsync(int notificationTemplateId)
        {
            var criteria = new GetTemplateByIdCriteria
            {
                NotificationTemplateId = notificationTemplateId
            };
            return await baseService.GetDataEntityAsync<NotificationTemplate>(criteria);
        }

        public async Task<CommandResult> DeleteAsync(int notificationTemplateId, int userId)
        {
            var command = new DeleteNotificationTemplateCommand
            {
                NotificationTemplateId = notificationTemplateId,
                UserId = userId
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}