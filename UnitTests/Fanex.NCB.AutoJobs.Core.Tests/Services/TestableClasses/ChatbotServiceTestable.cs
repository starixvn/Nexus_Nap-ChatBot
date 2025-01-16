namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models.Configurations;
    using Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Microsoft.Extensions.Options;

    public class ChatbotServiceTestable : ChatbotService
    {
        public ChatbotServiceTestable(
           IHttpClientFactory httpClientFactory,
           IOptions<AppSettings> appSettings,
           IOptions<BotSettings> botSettings,
           IOptions<MonitorChatbot> monitorChatbot,
           IOptions<NotifyChatGroupOption> notifyChatGroupOptions,
           IOptions<MonitorTelegramBot> monitorTelegramBot)
        : base(httpClientFactory, appSettings, botSettings, monitorChatbot, notifyChatGroupOptions, monitorTelegramBot)
        {
        }

        public async Task<HttpResponseMessage> Public_SendSingleMessageAsync(byte channelId, ConversationInfo conversationInfo)
        {
            return await base.SendSingleMessageAsync(channelId, conversationInfo);
        }

        public async Task<ChatbotToken> Public_GetTokenAsync()
        {
            return await base.GetTokenAsync();
        }
    }
}
