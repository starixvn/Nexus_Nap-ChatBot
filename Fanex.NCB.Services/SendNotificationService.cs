namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;

        public SendNotificationService(IChatbotService chatbotService, IBaseService baseService)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
        }

        public Task SendMessageToRecipients()
        {
            throw new System.NotImplementedException();
        }

        public async Task<CommandResult> UpdateNotificationSendingStatus(int notificationId, int creatorId)
        {
            var command = new UpdateNotificationSendingStatusCommand
            {
                NotificationId = notificationId,
                Creator = creatorId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private async Task<IEnumerable<int>> GetNotificationIdsToSend()
        {
            var criteria = new GetNotificationIdsToSendCriteria();
            return await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false);
        }
    }
}