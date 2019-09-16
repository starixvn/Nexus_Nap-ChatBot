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
            }).ConfigureAwait(false);

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<CommandResult> CreateAsync(CreateNotificationTemplate model)
        {
            var command = new CreateNotificationTemplateCommand
            {
                NotificationTypeId = model.NotificationTypeId,
                NotificationTemplateName = model.NotificationTemplateName,
                Title = model.Title,
                Content = model.Content,
                CreatorId = model.CreatorId
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public async Task<CommandResult> EditAsync(EditNotificationTemplate model)
        {
            var command = new EditNotificationTemplateCommand
            {
                NotificationTemplateId = model.NotificationTemplateId,
                NotificationTypeId = model.NotificationTypeId,
                NotificationTemplateName = model.NotificationTemplateName,
                Title = model.Title,
                Content = model.Content,
                ModifierId = model.ModifierId
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public async Task<NotificationTemplate> GetByIdAsync(int notificationTemplateId)
        {
            var result = await baseService.GetDataEntityAsync<NotificationTemplate>(new GetTemplateByIdCriteria
            {
                NotificationTemplateId = notificationTemplateId
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<CommandResult> DeleteAsync(int notificationTemplateId, int userId)
        {
            var command = new DeleteNotificationTemplateCommand
            {
                NotificationTemplateId = notificationTemplateId,
                UserId = userId
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }
    }
}