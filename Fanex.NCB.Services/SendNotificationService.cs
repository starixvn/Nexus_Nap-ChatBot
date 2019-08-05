namespace Fanex.NCB.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;

        public SendNotificationService(IChatbotService chatbotService)
        {
            this.chatbotService = chatbotService;
        }

        public Task SendMessageToRecipients()
        {
            throw new System.NotImplementedException();
        }
    }
}